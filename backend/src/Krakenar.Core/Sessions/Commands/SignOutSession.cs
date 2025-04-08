using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Commands;

public record SignOutSession(Guid Id) : ICommand<SessionDto?>;

public class SignOutSessionHandler : ICommandHandler<SignOutSession, SessionDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISessionQuerier SessionQuerier { get; }
  protected virtual ISessionRepository SessionRepository { get; }

  public SignOutSessionHandler(IApplicationContext applicationContext, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository)
  {
    ApplicationContext = applicationContext;
    SessionQuerier = sessionQuerier;
    SessionRepository = sessionRepository;
  }

  public virtual async Task<SessionDto?> HandleAsync(SignOutSession command, CancellationToken cancellationToken)
  {
    SessionId sessionId = new(command.Id, ApplicationContext.RealmId);
    Session? session = await SessionRepository.LoadAsync(sessionId, cancellationToken);
    if (session is null)
    {
      return null;
    }

    session.SignOut(ApplicationContext.ActorId);

    await SessionRepository.SaveAsync(session, cancellationToken);

    return await SessionQuerier.ReadAsync(session, cancellationToken);
  }
}
