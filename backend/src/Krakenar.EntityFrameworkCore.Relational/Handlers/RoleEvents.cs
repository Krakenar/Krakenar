using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles;
using Krakenar.Core.Roles.Events;
using Microsoft.EntityFrameworkCore;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using RoleEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Role;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class RoleEvents : IEventHandler<RoleCreated>, IEventHandler<RoleDeleted>, IEventHandler<RoleUniqueNameChanged>, IEventHandler<RoleUpdated>
{
  protected virtual KrakenarContext Context { get; }

  public RoleEvents(KrakenarContext context)
  {
    Context = context;
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
      // TODO(fpion): report
    }
    else
    {
      // TODO(fpion): report
    }
  }

  public virtual async Task HandleAsync(RoleDeleted @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await Context.Roles.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (role is null)
    {
      // TODO(fpion): report
    }
    else
    {
      Context.Roles.Remove(role);

      await Context.SaveChangesAsync(cancellationToken);
      // TODO(fpion): report
    }
  }

  public virtual async Task HandleAsync(RoleUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await Context.Roles.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (role is null)
    {
      return; // TODO(fpion): report
    }

    long expectedVersion = @event.Version - 1;
    if (role.Version < expectedVersion)
    {
      return; // TODO(fpion): report
    }
    else if (role.Version > expectedVersion)
    {
      return; // TODO(fpion): report
    }

    role.SetUniqueName(@event);

    await Context.SaveChangesAsync(cancellationToken);
    // TODO(fpion): report
  }

  public virtual async Task HandleAsync(RoleUpdated @event, CancellationToken cancellationToken)
  {
    RoleEntity? role = await Context.Roles.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (role is null)
    {
      return; // TODO(fpion): report
    }

    long expectedVersion = @event.Version - 1;
    if (role.Version < expectedVersion)
    {
      return; // TODO(fpion): report
    }
    else if (role.Version > expectedVersion)
    {
      return; // TODO(fpion): report
    }

    role.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);
    // TODO(fpion): report
  }
}
