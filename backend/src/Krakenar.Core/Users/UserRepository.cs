using Logitar.EventSourcing;

namespace Krakenar.Core.Users;

public interface IUserRepository
{
  Task SaveAsync(User user, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
}

public class UserRepository : Repository, IUserRepository
{
  public UserRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task SaveAsync(User user, CancellationToken cancellationToken)
  {
    await base.SaveAsync(user, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<User> users, CancellationToken cancellationToken)
  {
    await base.SaveAsync(users, cancellationToken);
  }
}
