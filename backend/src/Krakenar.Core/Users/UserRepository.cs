using Logitar.EventSourcing;

namespace Krakenar.Core.Users;

public interface IUserRepository
{
  Task<User?> LoadAsync(UserId id, CancellationToken cancellationToken = default);
  Task<User?> LoadAsync(UserId id, long? version, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<User>> LoadAsync(IEnumerable<UserId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(User user, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
}

public class UserRepository : Repository, IUserRepository
{
  public UserRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<User?> LoadAsync(UserId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<User?> LoadAsync(UserId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<User>(id.StreamId, version, cancellationToken);
  }
  public async Task<IReadOnlyCollection<User>> LoadAsync(IEnumerable<UserId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync<User>(ids.Select(id => id.StreamId), cancellationToken);
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
