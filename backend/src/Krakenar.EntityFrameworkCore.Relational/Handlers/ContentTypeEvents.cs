using Krakenar.Core;
using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Realms;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ContentTypeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.ContentType;
using FieldDefinitionEntity = Krakenar.EntityFrameworkCore.Relational.Entities.FieldDefinition;
using FieldTypeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.FieldType;
using ICommand = Logitar.Data.ICommand;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class ContentTypeEvents : IEventHandler<ContentTypeCreated>,
  IEventHandler<ContentTypeDeleted>,
  IEventHandler<ContentTypeFieldChanged>,
  IEventHandler<ContentTypeFieldRemoved>,
  IEventHandler<ContentTypeUniqueNameChanged>,
  IEventHandler<ContentTypeUpdated>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<ContentTypeEvents> Logger { get; }
  protected virtual ISqlHelper SqlHelper { get; }

  public ContentTypeEvents(KrakenarContext context, ILogger<ContentTypeEvents> logger, ISqlHelper sqlHelper)
  {
    Context = context;
    Logger = logger;
    SqlHelper = sqlHelper;
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

  public virtual async Task HandleAsync(ContentTypeFieldChanged @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await Context.ContentTypes
      .Include(x => x.FieldDefinitions)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType is null || contentType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, contentType);
      return;
    }

    FieldTypeEntity fieldType = await Context.FieldTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.Field.FieldTypeId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The field type entity 'StreamId={@event.Field.FieldTypeId}' could not be found.");

    FieldDefinitionEntity fieldDefinition = contentType.SetField(fieldType, @event);

    await Context.SaveChangesAsync(cancellationToken);

    ICommand command = SqlHelper.Update()
      .Set(new Update(KrakenarDb.FieldIndex.FieldDefinitionName, fieldDefinition.UniqueNameNormalized))
      .Where(new OperatorCondition(KrakenarDb.FieldIndex.FieldDefinitionId, Operators.IsEqualTo(fieldDefinition.FieldDefinitionId)))
      .Build();
    await Context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

    command = SqlHelper.Update()
      .Set(new Update(KrakenarDb.UniqueIndex.FieldDefinitionName, fieldDefinition.UniqueNameNormalized))
      .Where(new OperatorCondition(KrakenarDb.UniqueIndex.FieldDefinitionId, Operators.IsEqualTo(fieldDefinition.FieldDefinitionId)))
      .Build();
    await Context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ContentTypeFieldRemoved @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await Context.ContentTypes
      .Include(x => x.FieldDefinitions)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType is null || contentType.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, contentType);
      return;
    }

    FieldDefinitionEntity? fieldDefinition = contentType.RemoveField(@event);
    if (fieldDefinition is not null)
    {
      Context.FieldDefinitions.Remove(fieldDefinition);
    }

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
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

    ICommand command = SqlHelper.Update()
      .Set(new Update(KrakenarDb.PublishedContents.ContentTypeName, contentType.UniqueNameNormalized))
      .Where(new OperatorCondition(KrakenarDb.PublishedContents.ContentTypeId, Operators.IsEqualTo(contentType.ContentTypeId)))
      .Build();
    await Context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

    command = SqlHelper.Update()
      .Set(new Update(KrakenarDb.FieldIndex.ContentTypeName, contentType.UniqueNameNormalized))
      .Where(new OperatorCondition(KrakenarDb.FieldIndex.ContentTypeId, Operators.IsEqualTo(contentType.ContentTypeId)))
      .Build();
    await Context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

    command = SqlHelper.Update()
      .Set(new Update(KrakenarDb.UniqueIndex.ContentTypeName, contentType.UniqueNameNormalized))
      .Where(new OperatorCondition(KrakenarDb.UniqueIndex.ContentTypeId, Operators.IsEqualTo(contentType.ContentTypeId)))
      .Build();
    await Context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

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
