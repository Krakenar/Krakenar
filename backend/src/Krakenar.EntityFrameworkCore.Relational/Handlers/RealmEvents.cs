using Krakenar.Core;
using Krakenar.Core.Realms.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class RealmEvents : IEventHandler<RealmCreated>, IEventHandler<RealmDeleted>, IEventHandler<RealmUniqueSlugChanged>, IEventHandler<RealmUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<RealmEvents> Logger { get; }

  public RealmEvents(KrakenarContext context, ILogger<RealmEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(RealmCreated @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await Context.Realms.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm is null)
    {
      realm = new RealmEntity(@event);

      Context.Realms.Add(realm);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, realm);
    }
  }

  public virtual async Task HandleAsync(RealmDeleted @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Realms.Remove(realm);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(RealmUniqueSlugChanged @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm is null || realm.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, realm);
      return;
    }

    realm.SetUniqueSlug(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(RealmUpdated @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm is null || realm.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, realm);
      return;
    }

    realm.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
