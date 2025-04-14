using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Realms;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using ActorDto = Krakenar.Contracts.Actors.Actor;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class RealmQuerier : IRealmQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual DbSet<Entities.Realm> Realms { get; }

  public RealmQuerier(IActorService actorService, KrakenarContext context)
  {
    ActorService = actorService;
    Realms = context.Realms;
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
