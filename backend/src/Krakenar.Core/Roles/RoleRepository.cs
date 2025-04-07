using Logitar.EventSourcing;

namespace Krakenar.Core.Roles;

public interface IRoleRepository
{
  Task<Role?> LoadAsync(RoleId id, CancellationToken cancellationToken = default);
  Task<Role?> LoadAsync(RoleId id, long? version, CancellationToken cancellationToken = default);
}

public class RoleRepository : Repository, IRoleRepository
{
  public RoleRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Role?> LoadAsync(RoleId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Role?> LoadAsync(RoleId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Role>(id.StreamId, version, cancellationToken);
  }
}
