using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Passwords;
using Krakenar.Core.Sessions.Validators;
using Logitar.CQRS;
using Logitar.EventSourcing;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Commands;

public record RenewSession(RenewSessionPayload Payload) : ICommand<SessionDto>;

/// <exception cref="InvalidRefreshTokenException"></exception>
/// <exception cref="SessionNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class RenewSessionHandler : ICommandHandler<RenewSession, SessionDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IPasswordManager PasswordManager { get; }
  protected virtual ISessionQuerier SessionQuerier { get; }
  protected virtual ISessionRepository SessionRepository { get; }

  public RenewSessionHandler(
    IApplicationContext applicationContext,
    IPasswordManager passwordManager,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository)
  {
    ApplicationContext = applicationContext;
    PasswordManager = passwordManager;
    SessionQuerier = sessionQuerier;
    SessionRepository = sessionRepository;
  }

  public virtual async Task<SessionDto> HandleAsync(RenewSession command, CancellationToken cancellationToken)
  {
    RenewSessionPayload payload = command.Payload;
    new RenewSessionValidator().ValidateAndThrow(payload);

    RefreshToken refreshToken;
    try
    {
      refreshToken = RefreshToken.Decode(payload.RefreshToken, ApplicationContext.RealmId);
    }
    catch (Exception innerException)
    {
      throw new InvalidRefreshTokenException(payload.RefreshToken, nameof(payload.RefreshToken), innerException);
    }

    Session session = await SessionRepository.LoadAsync(refreshToken.SessionId, cancellationToken)
      ?? throw new SessionNotFoundException(refreshToken.SessionId, nameof(payload.RefreshToken));

    ActorId actorId = ApplicationContext.ActorId ?? new(session.UserId.Value);
    Password newSecret = PasswordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    session.Renew(refreshToken.Secret, newSecret, actorId);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      session.SetCustomAttribute(key, customAttribute.Value);
    }
    session.Update(actorId);

    await SessionRepository.SaveAsync(session, cancellationToken);

    SessionDto dto = await SessionQuerier.ReadAsync(session, cancellationToken);
    dto.RefreshToken = RefreshToken.Encode(session, secretString);
    return dto;
  }
}
