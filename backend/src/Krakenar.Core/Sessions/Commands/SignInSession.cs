﻿using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions.Validators;
using Krakenar.Core.Users;
using Logitar;
using Logitar.EventSourcing;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Commands;

public record SignInSession(SignInSessionPayload Payload) : ICommand<SessionDto>, ISensitiveActivity
{
  public IActivity Anonymize()
  {
    SignInSession clone = this.DeepClone();
    clone.Payload.Password = Payload.Password.Mask();
    return clone;
  }
}

/// <exception cref="IdAlreadyUsedException"></exception>
/// <exception cref="IncorrectUserPasswordException"></exception>
/// <exception cref="UserHasNoPasswordException"></exception>
/// <exception cref="UserIsDisabledException"></exception>
/// <exception cref="UserNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class SignInSessionHandler : ICommandHandler<SignInSession, SessionDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IPasswordManager PasswordManager { get; }
  protected virtual ISessionQuerier SessionQuerier { get; }
  protected virtual ISessionRepository SessionRepository { get; }
  protected virtual IUserManager UserManager { get; }

  public SignInSessionHandler(
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserManager userManager)
  {
    ApplicationContext = applicationContext;
    PasswordManager = passwordManager;
    SessionQuerier = sessionQuerier;
    SessionRepository = sessionRepository;
    UserManager = userManager;
  }

  public virtual async Task<SessionDto> HandleAsync(SignInSession command, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = command.Payload;
    new SignInSessionValidator().ValidateAndThrow(payload);

    RealmId? realmId = ApplicationContext.RealmId;
    Session? session;
    if (payload.Id.HasValue)
    {
      SessionId sessionId = new(payload.Id.Value, realmId);
      session = await SessionRepository.LoadAsync(sessionId, cancellationToken);
      if (session is not null)
      {
        throw new IdAlreadyUsedException<Session>(realmId, payload.Id.Value, nameof(payload.Id));
      }
    }

    FoundUsers users = await UserManager.FindAsync(payload.UniqueName, cancellationToken);
    User user = users.ByUniqueName ?? users.ByEmailAddress ?? throw new UserNotFoundException(realmId, payload.UniqueName, nameof(payload.UniqueName));
    if (ApplicationContext.RequireConfirmedAccount && !user.IsConfirmed)
    {
      throw new UserIsNotConfirmedException(user);
    }

    ActorId? actorId = ApplicationContext.ActorId;

    Password? secret = null;
    string? secretString = null;
    if (payload.IsPersistent)
    {
      secret = PasswordManager.GenerateBase64(RefreshToken.SecretLength, out secretString);
    }

    session = user.SignIn(payload.Password, secret, actorId, payload.Id);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      session.SetCustomAttribute(key, customAttribute.Value);
    }
    session.Update(actorId);

    await UserManager.SaveAsync(user, cancellationToken);
    await SessionRepository.SaveAsync(session, cancellationToken);

    SessionDto dto = await SessionQuerier.ReadAsync(session, cancellationToken);
    if (secretString is not null)
    {
      dto.RefreshToken = RefreshToken.Encode(session, secretString);
    }
    return dto;
  }
}
