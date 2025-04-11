using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles;
using Krakenar.Core.Roles.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using RoleEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Role;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class RoleEvents : IEventHandler<RoleCreated>, IEventHandler<RoleDeleted>, IEventHandler<RoleUniqueNameChanged>, IEventHandler<RoleUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<RoleEvents> Logger { get; }

  public RoleEvents(KrakenarContext context, ILogger<RoleEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(RoleCreated @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await Context.Roles.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (role is null)
    {
      RealmId? realmId = new RoleId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      role = new RoleEntity(realm, @event);

      Context.Roles.Add(role);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, role);
    }
  }

  public virtual async Task HandleAsync(RoleDeleted @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await Context.Roles.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (role is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Roles.Remove(role);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(RoleUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await Context.Roles.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (role is null || role.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, role);
      return;
    }

    role.SetUniqueName(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(RoleUpdated @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await Context.Roles.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (role is null || role.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, role);
      return;
    }

    role.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
