using Krakenar.Core.Realms.Events;

namespace Krakenar.Core.Realms;

public interface IRealmManager
{
  Task SaveAsync(Realm realm, CancellationToken cancellationToken = default);
}

public class RealmManager : IRealmManager
{
  protected virtual IRealmQuerier RealmQuerier { get; }
  protected virtual IRealmRepository RealmRepository { get; }

  public RealmManager(IRealmQuerier realmQuerier, IRealmRepository realmRepository)
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
