using Krakenar.Core.Sessions;
using Logitar.CQRS;
using Logitar.EventSourcing;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

public record SignOutUser(Guid Id) : ICommand<UserDto?>;

public class SignOutUserHandler : ICommandHandler<SignOutUser, UserDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISessionQuerier SessionQuerier { get; }
  protected virtual ISessionRepository SessionRepository { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }

  public SignOutUserHandler(
    IApplicationContext applicationContext,
    ISessionQuerier sessionQuerier,
    ISessionRepository sessionRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    ApplicationContext = applicationContext;
    SessionQuerier = sessionQuerier;
    SessionRepository = sessionRepository;
    UserQuerier = userQuerier;
    UserRepository = userRepository;
  }

  public virtual async Task<UserDto?> HandleAsync(SignOutUser command, CancellationToken cancellationToken)
  {
    UserId userId = new(command.Id, ApplicationContext.RealmId);
    User? user = await UserRepository.LoadAsync(userId, cancellationToken);
    if (user is null)
    {
      return null;
    }

    IReadOnlyCollection<SessionId> sessionIds = await SessionQuerier.FindActiveIdsAsync(user.Id, cancellationToken);
    if (sessionIds.Count > 0)
    {
      ActorId? actorId = ApplicationContext.ActorId;
      IReadOnlyCollection<Session> sessions = await SessionRepository.LoadAsync(sessionIds, cancellationToken);
      foreach (Session session in sessions)
      {
        session.SignOut(actorId);
      }
      await SessionRepository.SaveAsync(sessions, cancellationToken);
    }

    return await UserQuerier.ReadAsync(user, cancellationToken);
  }
}
