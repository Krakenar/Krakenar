using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ContentEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Content;
using ContentLocaleEntity = Krakenar.EntityFrameworkCore.Relational.Entities.ContentLocale;
using ContentTypeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.ContentType;
using FieldDefinitionEntity = Krakenar.EntityFrameworkCore.Relational.Entities.FieldDefinition;
using FieldIndexEntity = Krakenar.EntityFrameworkCore.Relational.Entities.FieldIndex;
using FieldTypeEntity = Krakenar.EntityFrameworkCore.Relational.Entities.FieldType;
using LanguageEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Language;
using RealmEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Realm;
using UniqueIndexEntity = Krakenar.EntityFrameworkCore.Relational.Entities.UniqueIndex;

namespace Krakenar.EntityFrameworkCore.Relational.Handlers;

public class ContentEvents : IEventHandler<ContentCreated>,
  IEventHandler<ContentDeleted>,
  IEventHandler<ContentLocaleChanged>,
  IEventHandler<ContentLocalePublished>,
  IEventHandler<ContentLocaleRemoved>,
  IEventHandler<ContentLocaleUnpublished>
{
  protected virtual KrakenarContext Context { get; }
  protected virtual ILogger<ContentTypeEvents> Logger { get; }

  public ContentEvents(KrakenarContext context, ILogger<ContentTypeEvents> logger)
  {
    Context = context;
    Logger = logger;
  }

  public virtual async Task HandleAsync(ContentCreated @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null)
    {
      ContentTypeEntity contentType = await Context.ContentTypes
        .Include(x => x.FieldDefinitions).ThenInclude(x => x.FieldType)
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.StreamId == @event.ContentTypeId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content type entity 'StreamId={@event.ContentTypeId}' could not be found.");

      content = new ContentEntity(contentType, @event);

      Context.Contents.Add(content);

      await Context.SaveChangesAsync(cancellationToken);

      await UpdateIndicesAsync(content.Locales.Single(), ContentStatus.Latest, cancellationToken);

      Logger.LogSuccess(@event);
    }
    else
    {
      Logger.LogUnexpectedVersion(@event, content);
    }
  }

  public virtual async Task HandleAsync(ContentDeleted @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null)
    {
      Logger.LogUnexpectedVersion(@event);
    }
    else
    {
      Context.Contents.Remove(content);

      await Context.SaveChangesAsync(cancellationToken);

      Logger.LogSuccess(@event);
    }
  }

  public virtual async Task HandleAsync(ContentLocaleChanged @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents
      .Include(x => x.ContentType).ThenInclude(x => x!.FieldDefinitions).ThenInclude(x => x.FieldType)
      .Include(x => x.ContentType).ThenInclude(x => x!.Realm)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null || content.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, content);
      return;
    }

    LanguageEntity? language = @event.LanguageId.HasValue
      ? (await Context.Languages.SingleOrDefaultAsync(x => x.StreamId == @event.LanguageId.Value.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.LanguageId}' could not be found."))
      : null;

    ContentLocaleEntity locale = content.SetLocale(language, @event);

    await Context.SaveChangesAsync(cancellationToken);

    await Context.FieldIndex
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ContentLocaleName, locale.UniqueNameNormalized), cancellationToken);

    await Context.UniqueIndex
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId)
      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ContentLocaleName, locale.UniqueNameNormalized), cancellationToken);

    await UpdateIndicesAsync(locale, ContentStatus.Latest, cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ContentLocalePublished @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents
      .Include(x => x.ContentType).ThenInclude(x => x!.FieldDefinitions).ThenInclude(x => x.FieldType)
      .Include(x => x.ContentType).ThenInclude(x => x!.Realm)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .Include(x => x.Locales).ThenInclude(x => x.PublishedContent)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null || content.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, content);
      return;
    }

    ContentLocaleEntity? locale = content.Publish(@event)
      ?? throw new InvalidOperationException($"The content 'StreamId={@event.StreamId}' locale 'LanguageId={@event.LanguageId}' could not be found.");

    await Context.SaveChangesAsync(cancellationToken);

    await UpdateIndicesAsync(locale, ContentStatus.Published, cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ContentLocaleRemoved @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null || content.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, content);
      return;
    }

    ContentLocaleEntity? locale = content.RemoveLocale(@event);
    if (locale is not null)
    {
      Context.ContentLocales.Remove(locale);
    }

    await Context.SaveChangesAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  public virtual async Task HandleAsync(ContentLocaleUnpublished @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await Context.Contents
      .Include(x => x.ContentType).ThenInclude(x => x!.FieldDefinitions).ThenInclude(x => x.FieldType)
      .Include(x => x.ContentType).ThenInclude(x => x!.Realm)
      .Include(x => x.Locales).ThenInclude(x => x.Language)
      .Include(x => x.Locales).ThenInclude(x => x.PublishedContent)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content is null || content.Version != (@event.Version - 1))
    {
      Logger.LogUnexpectedVersion(@event, content);
      return;
    }

    ContentLocaleEntity? locale = content.Unpublish(@event)
      ?? throw new InvalidOperationException($"The content 'StreamId={@event.StreamId}' locale 'LanguageId={@event.LanguageId}' could not be found.");

    if (locale.PublishedContent is not null)
    {
      Context.PublishedContents.Remove(locale.PublishedContent);
    }

    await Context.SaveChangesAsync(cancellationToken);

    await Context.FieldIndex
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId && x.Status == ContentStatus.Published)
      .ExecuteDeleteAsync(cancellationToken);

    await Context.UniqueIndex
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId && x.Status == ContentStatus.Published)
      .ExecuteDeleteAsync(cancellationToken);

    Logger.LogSuccess(@event);
  }

  protected virtual async Task UpdateIndicesAsync(ContentLocaleEntity locale, ContentStatus status, CancellationToken cancellationToken)
  {
    ContentEntity content = locale.Content ?? throw new ArgumentException("The content is required.", nameof(locale));
    LanguageEntity? language = locale.LanguageId.HasValue
      ? (locale.Language ?? throw new ArgumentException("The language is required.", nameof(locale)))
      : null;
    ContentTypeEntity contentType = content.ContentType ?? throw new ArgumentException("The content type is required.", nameof(locale));
    RealmEntity? realm = contentType.RealmId.HasValue
      ? (contentType.Realm ?? throw new ArgumentException("The realm is required.", nameof(locale)))
      : null;
    Dictionary<Guid, FieldDefinitionEntity> fieldDefinitions = contentType.FieldDefinitions.ToDictionary(x => x.Id, x => x);

    Dictionary<Guid, FieldIndexEntity> indexedFields = await Context.FieldIndex
      .Include(x => x.FieldType)
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId && x.Status == status)
      .ToDictionaryAsync(x => x.FieldDefinitionUid, x => x, cancellationToken);
    Dictionary<Guid, UniqueIndexEntity> uniqueFields = await Context.UniqueIndex
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId && x.Status == status)
      .ToDictionaryAsync(x => x.FieldDefinitionUid, x => x, cancellationToken);

    Dictionary<Guid, string> fieldValues = locale.GetFieldValues();

    foreach (KeyValuePair<Guid, FieldIndexEntity> indexedField in indexedFields)
    {
      if (!fieldValues.ContainsKey(indexedField.Key))
      {
        Context.FieldIndex.Remove(indexedField.Value);
      }
    }
    foreach (KeyValuePair<Guid, UniqueIndexEntity> uniqueField in uniqueFields)
    {
      if (!fieldValues.ContainsKey(uniqueField.Key))
      {
        Context.UniqueIndex.Remove(uniqueField.Value);
      }
    }

    long version = (status == ContentStatus.Published ? locale.PublishedVersion : null) ?? locale.Version;
    foreach (KeyValuePair<Guid, string> fieldValue in fieldValues)
    {
      FieldDefinitionEntity fieldDefinition = fieldDefinitions[fieldValue.Key];
      FieldTypeEntity fieldType = fieldDefinition.FieldType ?? throw new ArgumentException($"The field definition 'Id={fieldDefinition.Id}' did not include a field type.", nameof(locale));

      if (fieldDefinition.IsIndexed)
      {
        if (indexedFields.TryGetValue(fieldValue.Key, out FieldIndexEntity? indexedField))
        {
          indexedField.Update(version, fieldValue.Value);
        }
        else
        {
          indexedField = new(realm, contentType, language, fieldType, fieldDefinition, content, locale, status, fieldValue.Value);
          indexedFields[fieldValue.Key] = indexedField;

          Context.FieldIndex.Add(indexedField);
        }
      }

      if (fieldDefinition.IsUnique)
      {
        if (uniqueFields.TryGetValue(fieldValue.Key, out UniqueIndexEntity? uniqueField))
        {
          uniqueField.Update(version, fieldValue.Value);
        }
        else
        {
          uniqueField = new(realm, contentType, language, fieldType, fieldDefinition, content, locale, status, fieldValue.Value);
          uniqueFields[fieldValue.Key] = uniqueField;

          Context.UniqueIndex.Add(uniqueField);
        }
      }
    }

    await Context.SaveChangesAsync(cancellationToken);
  }
}
