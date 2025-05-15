using Krakenar.Core;
using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Realms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ContentTypeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.ContentType;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class ContentTypeEvents : IEventHandler<ContentTypeCreated>,
  IEventHandler<ContentTypeDeleted>,
  IEventHandler<ContentTypeUniqueNameChanged>,
  IEventHandler<ContentTypeUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<ContentTypeEvents> Logger { get; }

  public ContentTypeEvents(KrakenarContext context, ILogger<ContentTypeEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(ContentTypeCreated @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await Context.ContentTypes.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType is null)
    {
      RealmId? realmId = new ContentTypeId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      contentType = new ContentTypeEntity(realm, @event);

      Context.ContentTypes.Add(contentType);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, contentType);
    }
  }

  public virtual async Task HandleAsync(ContentTypeDeleted @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await Context.ContentTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.ContentTypes.Remove(contentType);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(ContentTypeUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await Context.ContentTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType is null || contentType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, contentType);
      return;
    }

    contentType.SetUniqueName(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ContentTypeUpdated @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await Context.ContentTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType is null || contentType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, contentType);
      return;
    }

    contentType.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
