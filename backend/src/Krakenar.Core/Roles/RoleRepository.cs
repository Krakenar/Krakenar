using Logitar.EventSourcing;

namespace Krakenar.Core.Roles;

public interface IRoleRepository
{
  Task<Role?> LoadAsync(RoleId id, CancellationToken cancellationToken = default);
  Task<Role?> LoadAsync(RoleId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Role role, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Role> roles, CancellationToken cancellationToken = default);
}

public class RoleRepository : Repository, IRoleRepository
{
  public RoleRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<Role?> LoadAsync(RoleId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<Role?> LoadAsync(RoleId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Role>(id.StreamId, version, cancellationToken);
  }

  public virtual async Task SaveAsync(Role role, CancellationToken cancellationToken)
  {
    await base.SaveAsync(role, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<Role> roles, CancellationToken cancellationToken)
  {
    await base.SaveAsync(roles, cancellationToken);
  }
}
