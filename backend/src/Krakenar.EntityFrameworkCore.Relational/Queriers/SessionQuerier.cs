using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Sessions;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Session = Krakenar.Core.Sessions.Session;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class SessionQuerier : ISessionQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.Session> Sessions { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public SessionQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Sessions = context.Sessions;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<SessionDto> ReadAsync(Session session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id, cancellationToken) ?? throw new InvalidOperationException($"The session entity 'StreamId={session.Id}' could not be found.");
  }
  public virtual async Task<SessionDto?> ReadAsync(SessionId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<SessionDto?> ReadAsync(Guid id, CancellationToken cancellationTokent)
  {
    Entities.Session? session = await Sessions.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationTokent);

    return session is null ? null : await MapAsync(session, cancellationTokent);
  }

  public virtual async Task<SearchResults<SessionDto>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.Sessions.Table).SelectAll(KrakenarDb.Sessions.Table)
      .Join(KrakenarDb.Users.UserId, KrakenarDb.Sessions.UserId)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.Sessions.RealmUid)
      .ApplyIdFilter(KrakenarDb.Sessions.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search);

    if (payload.UserId.HasValue)
    {
      builder.Where(KrakenarDb.Users.Id, Operators.IsEqualTo(payload.UserId.Value));
    }
    if (payload.IsActive.HasValue)
    {
      builder.Where(KrakenarDb.Sessions.IsActive, Operators.IsEqualTo(payload.IsActive.Value));
    }
    if (payload.IsPersistent.HasValue)
    {
      builder.Where(KrakenarDb.Sessions.IsPersistent, Operators.IsEqualTo(payload.IsPersistent.Value));
    }

    IQueryable<Entities.Session> query = Sessions.FromQuery(builder).AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.Identifiers)
      .Include(x => x.User).ThenInclude(x => x!.Roles);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.Session>? ordered = null;
    foreach (SessionSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case SessionSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case SessionSort.SignedOutOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.SignedOutOn) : query.OrderBy(x => x.SignedOutOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.SignedOutOn) : ordered.ThenBy(x => x.SignedOutOn));
          break;
        case SessionSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.Session[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<SessionDto> sessions = await MapAsync(entities, cancellationToken);

    return new SearchResults<SessionDto>(sessions, total);
  }

  protected virtual async Task<SessionDto> MapAsync(Entities.Session session, CancellationToken cancellationToken)
  {
    return (await MapAsync([session], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<SessionDto>> MapAsync(IEnumerable<Entities.Session> sessions, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return sessions.Select(session => mapper.ToSession(session, realm)).ToList().AsReadOnly();
  }
}
