using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Realms;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using ActorDto = Krakenar.Contracts.Actors.Actor;
using Realm = Krakenar.Core.Realms.Realm;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class RealmQuerier : IRealmQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual DbSet<Entities.Realm> Realms { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public RealmQuerier(IActorService actorService, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    Realms = context.Realms;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<RealmId?> FindIdAsync(Slug uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = Helper.Normalize(uniqueSlug);

    string? streamId = await Realms.AsNoTracking()
      .Where(x => x.UniqueSlugNormalized == uniqueSlugNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new RealmId(streamId);
  }

  public virtual async Task<RealmDto> ReadAsync(Realm realm, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm.Id, cancellationToken) ?? throw new InvalidOperationException($"The realm entity 'StreamId={realm.Id}' could not be found.");
  }
  public virtual async Task<RealmDto?> ReadAsync(RealmId id, CancellationToken cancellationToken)
  {
    Entities.Realm? realm = await Realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);

    return realm is null ? null : await MapAsync(realm, cancellationToken);
  }
  public virtual async Task<RealmDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.Realm? realm = await Realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return realm is null ? null : await MapAsync(realm, cancellationToken);
  }
  public virtual async Task<RealmDto?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken)
  {
    string uniqueSlugNormalized = Helper.Normalize(uniqueSlug);

    Entities.Realm? realm = await Realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.UniqueSlugNormalized == uniqueSlugNormalized, cancellationToken);

    return realm is null ? null : await MapAsync(realm, cancellationToken);
  }

  public virtual async Task<SearchResults<RealmDto>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.Realms.Table).SelectAll(KrakenarDb.Realms.Table)
      .ApplyIdFilter(KrakenarDb.Realms.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.Realms.UniqueSlug, KrakenarDb.Realms.DisplayName);

    IQueryable<Entities.Realm> query = Realms.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.Realm>? ordered = null;
    foreach (RealmSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RealmSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case RealmSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case RealmSort.UniqueSlug:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueSlug) : query.OrderBy(x => x.UniqueSlug))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueSlug) : ordered.ThenBy(x => x.UniqueSlug));
          break;
        case RealmSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.Realm[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<RealmDto> realms = await MapAsync(entities, cancellationToken);

    return new SearchResults<RealmDto>(realms, total);
  }

  protected virtual async Task<RealmDto> MapAsync(Entities.Realm realm, CancellationToken cancellationToken)
  {
    return (await MapAsync([realm], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<RealmDto>> MapAsync(IEnumerable<Entities.Realm> realms, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = realms.SelectMany(realm => realm.GetActorIds());
    IReadOnlyDictionary<ActorId, ActorDto> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return realms.Select(mapper.ToRealm).ToList().AsReadOnly();
  }
}
