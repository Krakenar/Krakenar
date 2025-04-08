using Krakenar.Core.Realms.Events;

namespace Krakenar.Core.Realms;

public interface IRealmService
{
  Task SaveAsync(Realm realm, CancellationToken cancellationToken = default);
}

public class RealmService : IRealmService
{
  protected virtual IRealmQuerier RealmQuerier { get; }
  protected virtual IRealmRepository RealmRepository { get; }

  public RealmService(IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    RealmQuerier = realmQuerier;
    RealmRepository = realmRepository;
  }

  public virtual async Task SaveAsync(Realm realm, CancellationToken cancellationToken)
  {
    bool hasUniqueSlugChanged = realm.Changes.Any(change => change is RealmCreated || change is RealmUniqueSlugChanged);
    if (hasUniqueSlugChanged)
    {
      RealmId? conflictId = await RealmQuerier.FindIdAsync(realm.UniqueSlug, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(realm.Id))
      {
        throw new UniqueSlugAlreadyUsedException(realm, conflictId.Value);
      }
    }

    await RealmRepository.SaveAsync(realm, cancellationToken);
  }
}
