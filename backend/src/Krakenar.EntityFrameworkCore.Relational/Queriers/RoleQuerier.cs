using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Roles;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Role = Krakenar.Core.Roles.Role;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class RoleQuerier : IRoleQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<Entities.Role> Roles { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public RoleQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context, ISqlHelper sqlHelper)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Roles = context.Roles;
    SqlHelper = sqlHelper;
  }

  public virtual async Task<RoleId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await Roles.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new RoleId(streamId);
  }

  public virtual async Task<RoleDto> ReadAsync(Role role, CancellationToken cancellationToken)
  {
    return await ReadAsync(role.Id, cancellationToken) ?? throw new InvalidOperationException($"The role entity 'StreamId={role.Id}' could not be found.");
  }
  public virtual async Task<RoleDto?> ReadAsync(RoleId id, CancellationToken cancellationToken)
  {
    if (id.RealmId != ApplicationContext.RealmId)
    {
      throw new NotSupportedException();
    }

    return await ReadAsync(id.EntityId, cancellationToken);
  }
  public virtual async Task<RoleDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Entities.Role? role = await Roles.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return role is null ? null : await MapAsync(role, cancellationToken);
  }
  public virtual async Task<RoleDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    Entities.Role? role = await Roles.AsNoTracking()
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return role is null ? null : await MapAsync(role, cancellationToken);
  }

  public virtual async Task<SearchResults<RoleDto>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = SqlHelper.Query(KrakenarDb.Roles.Table).SelectAll(KrakenarDb.Roles.Table)
      .WhereRealm(ApplicationContext.RealmId, KrakenarDb.Roles.RealmUid)
      .ApplyIdFilter(KrakenarDb.Roles.Id, payload.Ids);
    SqlHelper.ApplyTextSearch(builder, payload.Search, KrakenarDb.Roles.UniqueName, KrakenarDb.Roles.DisplayName);

    IQueryable<Entities.Role> query = Roles.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Entities.Role>? ordered = null;
    foreach (RoleSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RoleSort.CreatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case RoleSort.DisplayName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.DisplayName) : ordered.ThenBy(x => x.DisplayName));
          break;
        case RoleSort.UniqueName:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UniqueName) : ordered.ThenBy(x => x.UniqueName));
          break;
        case RoleSort.UpdatedOn:
          ordered = (ordered == null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Entities.Role[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<RoleDto> roles = await MapAsync(entities, cancellationToken);

    return new SearchResults<RoleDto>(roles, total);
  }

  protected virtual async Task<RoleDto> MapAsync(Entities.Role role, CancellationToken cancellationToken)
  {
    return (await MapAsync([role], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<RoleDto>> MapAsync(IEnumerable<Entities.Role> roles, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = roles.SelectMany(role => role.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return roles.Select(role => mapper.ToRole(role, realm)).ToList().AsReadOnly();
  }
}
