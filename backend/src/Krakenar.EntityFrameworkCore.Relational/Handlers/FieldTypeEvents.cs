using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Events;
using Krakenar.Core.Realms;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FieldTypeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.FieldType;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class FieldTypeEvents : IEventHandler<FieldTypeBooleanSettingsChanged>,
  IEventHandler<FieldTypeCreated>,
  IEventHandler<FieldTypeDateTimeSettingsChanged>,
  IEventHandler<FieldTypeDeleted>,
  IEventHandler<FieldTypeNumberSettingsChanged>,
  IEventHandler<FieldTypeRelatedContentSettingsChanged>,
  IEventHandler<FieldTypeRichTextSettingsChanged>,
  IEventHandler<FieldTypeSelectSettingsChanged>,
  IEventHandler<FieldTypeStringSettingsChanged>,
  IEventHandler<FieldTypeTagsSettingsChanged>,
  IEventHandler<FieldTypeUniqueNameChanged>,
  IEventHandler<FieldTypeUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<FieldTypeEvents> Logger { get; }

  public FieldTypeEvents(KrakenarContext context, ILogger<FieldTypeEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(FieldTypeBooleanSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    fieldType.SetSettings(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(FieldTypeCreated @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null)
    {
      RealmId? realmId = new FieldTypeId(@event.StreamId).RealmId;
      RealmEntity? realm = realmId.HasValue
        ? (await Context.Realms.SingleOrDefaultAsync(x => x.StreamId == realmId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm entity 'StreamId={realmId}' could not be found."))
        : null;

      fieldType = new FieldTypeEntity(realm, @event);

      Context.FieldTypes.Add(fieldType);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
    }
  }

  public virtual async Task HandleAsync(FieldTypeDateTimeSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    fieldType.SetSettings(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(FieldTypeDeleted @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.FieldTypes.Remove(fieldType);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(FieldTypeNumberSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    fieldType.SetSettings(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(FieldTypeRelatedContentSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    Entities.ContentType relatedContentType = await Context.ContentTypes
      .SingleOrDefaultAsync(x => x.RealmId == fieldType.RealmId && x.Id == @event.Settings.ContentTypeId, cancellationToken)
      ?? throw new InvalidOperationException($"The content type entity 'RealmId={fieldType.RealmId}, Id={@event.Settings.ContentTypeId}' could not be found.");

    fieldType.SetSettings(relatedContentType, @event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(FieldTypeRichTextSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    fieldType.SetSettings(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(FieldTypeSelectSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    fieldType.SetSettings(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(FieldTypeStringSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    fieldType.SetSettings(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(FieldTypeTagsSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    fieldType.SetSettings(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(FieldTypeUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    fieldType.SetUniqueName(@event);

    await Context.SaveChangesAsync(cancellationToken);

    await Context.FieldIndex
      .Where(x => x.FieldTypeId == fieldType.FieldTypeId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.FieldTypeName, fieldType.UniqueNameNormalized), cancellationToken);

    await Context.UniqueIndex
      .Where(x => x.FieldTypeId == fieldType.FieldTypeId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.FieldTypeName, fieldType.UniqueNameNormalized), cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(FieldTypeUpdated @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await Context.FieldTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType is null || fieldType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, fieldType);
      return;
    }

    fieldType.Update(@event);

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }
}
