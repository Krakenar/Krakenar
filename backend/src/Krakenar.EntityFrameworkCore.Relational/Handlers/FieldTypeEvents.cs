using Krakenar.Core;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Events;
using Krakenar.Core.Realms;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FieldTypeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.FieldType;
using ICommand = Logitar.Data.ICommand;
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
  protected virtual ISqlHelper SqlHelper { get; }

  public FieldTypeEvents(KrakenarContext context, ILogger<FieldTypeEvents> logger, ISqlHelper sqlHelper)
  {
    Context = context;
    Logger = logger;
    SqlHelper = sqlHelper;
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

    fieldType.SetSettings(@event);

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

    ICommand command = SqlHelper.Update()
      .Set(new Update(KrakenarDb.FieldIndex.FieldTypeName, fieldType.UniqueNameNormalized))
      .Where(new OperatorCondition(KrakenarDb.FieldIndex.FieldTypeId, Operators.IsEqualTo(fieldType.FieldTypeId)))
      .Build();
    await Context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

    command = SqlHelper.Update()
      .Set(new Update(KrakenarDb.UniqueIndex.FieldTypeName, fieldType.UniqueNameNormalized))
      .Where(new OperatorCondition(KrakenarDb.UniqueIndex.FieldTypeId, Operators.IsEqualTo(fieldType.FieldTypeId)))
      .Build();
    await Context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

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
