using Krakenar.Core;
using Krakenar.Core.Realms.Events;
using Microsoft.EntityFrameworkCore;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class RealmEvents : IEventHandler<RealmCreated>, IEventHandler<RealmDeleted>, IEventHandler<RealmUniqueSlugChanged>, IEventHandler<RealmUpdated>
{
  protected KrakenarContext Context { get; }

  public RealmEvents(KrakenarContext context)
  {
    Context = context;
  }

  public virtual async Task HandleAsync(RealmCreated @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await Context.Realms.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm is null)
    {
      realm = new RealmEntity(@event);

      Context.Realms.Add(realm);

      await Context.SaveChangesAsync(cancellationToken);
      // TODO(fpion): report
    }
    else
    {
      // TODO(fpion): report
    }
  }

  public virtual async Task HandleAsync(RealmDeleted @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm is null)
    {
      // TODO(fpion): report
    }
    else
    {
      Context.Realms.Remove(realm);

      await Context.SaveChangesAsync(cancellationToken);
      // TODO(fpion): report
    }
  }

  public virtual async Task HandleAsync(RealmUniqueSlugChanged @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm is null)
    {
      return; // TODO(fpion): report
    }

    long expectedVersion = @event.Version - 1;
    if (realm.Version < expectedVersion)
    {
      return; // TODO(fpion): report
    }
    else if (realm.Version > expectedVersion)
    {
      return; // TODO(fpion): report
    }

    realm.SetUniqueSlug(@event);

    await Context.SaveChangesAsync(cancellationToken);
    // TODO(fpion): report
  }

  public virtual async Task HandleAsync(RealmUpdated @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm is null)
    {
      return; // TODO(fpion): report
    }

    long expectedVersion = @event.Version - 1;
    if (realm.Version < expectedVersion)
    {
      return; // TODO(fpion): report
    }
    else if (realm.Version > expectedVersion)
    {
      return; // TODO(fpion): report
    }

    realm.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);
    // TODO(fpion): report
  }
}
