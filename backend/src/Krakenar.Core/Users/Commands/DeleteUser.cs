using Krakenar.Core.Sessions;
using Logitar.EventSourcing;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

public record DeleteUser(Guid Id) : ICommand<UserDto?>;

public class DeleteUserHandler : ICommandHandler<DeleteUser, UserDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISessionQuerier SessionQuerier { get; }
  protected virtual ISessionRepository SessionRepository { get; }
  protected virtual IUserQuerier UserQuerier { get; }
  protected virtual IUserRepository UserRepository { get; }

  public DeleteUserHandler(
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

  public virtual async Task<UserDto?> HandleAsync(DeleteUser command, CancellationToken cancellationToken)
  {
    UserId userId = new(command.Id, ApplicationContext.RealmId);
    User? user = await UserRepository.LoadAsync(userId, cancellationToken);
    if (user is null)
    {
      return null;
    }
    UserDto dto = await UserQuerier.ReadAsync(user, cancellationToken);

    ActorId? actorId = ApplicationContext.ActorId;

    // TODO(fpion): delete OTPs

    IReadOnlyCollection<SessionId> sessionIds = await SessionQuerier.FindIdsAsync(user.Id, cancellationToken);
    if (sessionIds.Count > 0)
    {
      IReadOnlyCollection<Session> sessions = await SessionRepository.LoadAsync(sessionIds, cancellationToken);
      foreach (Session session in sessions)
      {
        session.Delete(actorId);
      }
      await SessionRepository.SaveAsync(sessions, cancellationToken);
    }

    user.Delete(actorId);
    await UserRepository.SaveAsync(user, cancellationToken);

    return dto;
  }
}
