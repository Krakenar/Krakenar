using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions.Validators;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Commands;

public record CreateSession(CreateSessionPayload Payload) : ICommand<SessionDto>;

/// <exception cref="IdAlreadyUsedException"></exception>
/// <exception cref="UserIsDisabledException"></exception>
/// <exception cref="UserNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateSessionHandler : ICommandHandler<CreateSession, SessionDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IPasswordService PasswordService { get; }
  protected virtual ISessionQuerier SessionQuerier { get; }
  protected virtual ISessionRepository SessionRepository { get; }
  protected virtual IUserService UserService { get; }

  public CreateSessionHandler(
    IApplicationContext applicationContext,
    IPasswordService passwordService,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserService userService)
  {
    ApplicationContext = applicationContext;
    PasswordService = passwordService;
    SessionQuerier = sessionQuerier;
    SessionRepository = sessionRepository;
    UserService = userService;
  }

  public virtual async Task<SessionDto> HandleAsync(CreateSession command, CancellationToken cancellationToken)
  {
    CreateSessionPayload payload = command.Payload;
    new CreateSessionValidator().ValidateAndThrow(payload);

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

    FoundUsers users = await UserService.FindAsync(payload.User, cancellationToken);
    User user = users.ById ?? users.ByUniqueName ?? users.ByEmailAddress ?? users.ByCustomIdentifier
      ?? throw new UserNotFoundException(realmId, payload.User, nameof(payload.User));

    ActorId? actorId = ApplicationContext.ActorId;

    Password? secret = null;
    string? secretString = null;
    if (payload.IsPersistent)
    {
      secret = PasswordService.GenerateBase64(RefreshToken.SecretLength, out secretString);
    }

    session = user.SignIn(secret, actorId, payload.Id);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      session.SetCustomAttribute(key, customAttribute.Value);
    }
    session.Update(actorId);

    await UserService.SaveAsync(user, cancellationToken);
    await SessionRepository.SaveAsync(session, cancellationToken);

    SessionDto dto = await SessionQuerier.ReadAsync(session, cancellationToken);
    if (secretString is not null)
    {
      dto.RefreshToken = RefreshToken.Encode(session, secretString);
    }
    return dto;
  }
}
