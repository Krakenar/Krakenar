using Logitar.EventSourcing;

namespace Krakenar.Core.Realms;

public interface IRealmRepository
{
  Task<Realm?> LoadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<Realm?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Realm realm, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Realm> realms, CancellationToken cancellationToken = default);
}

public class RealmRepository : Repository, IRealmRepository
{
  public RealmRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<Realm?> LoadAsync(RealmId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<Realm?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Realm>(id.StreamId, version, cancellationToken);
  }

  public virtual async Task SaveAsync(Realm realm, CancellationToken cancellationToken)
  {
    await base.SaveAsync(realm, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<Realm> realms, CancellationToken cancellationToken)
  {
    await base.SaveAsync(realms, cancellationToken);
  }
}
