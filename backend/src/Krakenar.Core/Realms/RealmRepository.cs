using Logitar.EventSourcing;

namespace Krakenar.Core.Realms;

public interface IRealmRepository
{
  Task<Realm?> LoadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<Realm?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken = default);
}

public class RealmRepository : Repository, IRealmRepository
{
  public RealmRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Realm?> LoadAsync(RealmId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Realm?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Realm>(id.StreamId, version, cancellationToken);
  }
}
