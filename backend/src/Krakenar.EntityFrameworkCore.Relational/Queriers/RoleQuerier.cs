using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Realms;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.Roles;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using RoleDto = Krakenar.Contracts.Roles.Role;
using RoleEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Role;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class RoleQuerier : IRoleQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual DbSet<RoleEntity> Roles { get; }

  public RoleQuerier(IActorService actorService, IApplicationContext applicationContext, KrakenarContext context)
  {
    ActorService = actorService;
    ApplicationContext = applicationContext;
    Roles = context.Roles;
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
    RoleEntity? role = await Roles.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);

    return role is null ? null : await MapAsync(role, cancellationToken); // TODO(fpion): will not work if entity is different realm than application context!
  }
  public virtual async Task<RoleDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RoleEntity? role = await Roles.AsNoTracking()
      .Include(x => x.Realm)
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return role is null ? null : await MapAsync(role, cancellationToken);
  }
  public virtual async Task<RoleDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    RoleEntity? role = await Roles.AsNoTracking()
      .Include(x => x.Realm)
      .WhereRealm(ApplicationContext.RealmId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return role is null ? null : await MapAsync(role, cancellationToken);
  }

  protected virtual async Task<RoleDto> MapAsync(RoleEntity role, CancellationToken cancellationToken)
  {
    return (await MapAsync([role], cancellationToken)).Single();
  }
  protected virtual async Task<IReadOnlyCollection<RoleDto>> MapAsync(IEnumerable<RoleEntity> roles, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = roles.SelectMany(role => role.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    Realm? realm = ApplicationContext.Realm;
    return roles.Select(role => mapper.ToRole(role, realm)).ToList().AsReadOnly();
  }
}
