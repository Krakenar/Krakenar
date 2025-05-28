using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Validators;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents;

public interface IContentManager
{
  Task<ContentType> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken = default);
  Task SaveAsync(Content content, ContentType? contentType = null, CancellationToken cancellationToken = default);
  Task SaveAsync(ContentType contentType, CancellationToken cancellationToken = default);
}

public class ContentManager : IContentManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IContentRepository ContentRepository { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }
  protected virtual IFieldTypeRepository FieldTypeRepository { get; }
  protected virtual IFieldValueValidatorFactory FieldValueValidatorFactory { get; }

  public ContentManager(
    IApplicationContext applicationContext,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository,
    IFieldTypeRepository fieldTypeRepository,
    IFieldValueValidatorFactory fieldValueValidatorFactory)
  {
    ApplicationContext = applicationContext;
    ContentQuerier = contentQuerier;
    ContentRepository = contentRepository;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
    FieldTypeRepository = fieldTypeRepository;
    FieldValueValidatorFactory = fieldValueValidatorFactory;
  }

  public virtual async Task<ContentType> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken)
  {
    RealmId? realmId = ApplicationContext.RealmId;
    ContentType? contentType = null;

    if (Guid.TryParse(idOrUniqueName, out Guid entityId))
    {
      ContentTypeId contentTypeId = new(entityId, realmId);
      contentType = await ContentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    }

    if (contentType is null)
    {
      try
      {
        Identifier uniqueName = new(idOrUniqueName);
        ContentTypeId? contentTypeId = await ContentTypeQuerier.FindIdAsync(uniqueName, cancellationToken);
        if (contentTypeId.HasValue)
        {
          contentType = await ContentTypeRepository.LoadAsync(contentTypeId.Value, cancellationToken);
        }
      }
      catch (ValidationException)
      {
      }
    }

    return contentType ?? throw new ContentTypeNotFoundException(realmId, idOrUniqueName, propertyName);
  }

  public virtual async Task SaveAsync(Content content, ContentType? contentType, CancellationToken cancellationToken)
  {
    HashSet<LanguageId?> languageIds = [];
    foreach (IEvent @event in content.Changes)
    {
      if (@event is ContentCreated)
      {
        languageIds.Add(null);
      }
      else if (@event is ContentLocaleChanged changed)
      {
        if (changed.LanguageId.HasValue)
        {
          languageIds.Add(changed.LanguageId.Value);
        }
        else
        {
          languageIds.Add(null);
        }
      }
      else if (@event is ContentLocalePublished published)
      {
        if (published.LanguageId.HasValue)
        {
          languageIds.Add(published.LanguageId.Value);
        }
        else
        {
          languageIds.Add(null);
        }
      }
    }

    if (languageIds.Count > 0)
    {
      contentType ??= await ContentTypeRepository.LoadAsync(content.ContentTypeId, cancellationToken)
        ?? throw new InvalidOperationException($"The content type 'Id={content.ContentTypeId}' was not loaded.");

      HashSet<FieldTypeId> fieldTypeIds = [.. contentType.Fields.Select(field => field.FieldTypeId)];
      Dictionary<FieldTypeId, FieldType> fieldTypes = fieldTypeIds.Count == 0
        ? []
        : (await FieldTypeRepository.LoadAsync(fieldTypeIds, cancellationToken)).ToDictionary(x => x.Id, x => x);

      foreach (LanguageId? languageId in languageIds)
      {
        ContentLocale locale = languageId.HasValue ? content.FindLocale(languageId.Value) : content.Invariant;
        UniqueName uniqueName = locale.UniqueName;
        ContentId? conflictId = await ContentQuerier.FindIdAsync(content.ContentTypeId, languageId, uniqueName, cancellationToken);
        if (conflictId.HasValue && !conflictId.Value.Equals(content.Id))
        {
          throw new ContentUniqueNameAlreadyUsedException(content, languageId, conflictId.Value, uniqueName);
        }

        bool isPublishing = content.GetStatus(languageId) == ContentStatus.Latest; // NOTE(fpion): we only check Required fields when publishing content, not when saving drafts.
        await ValidateAsync(contentType, fieldTypes, content.Id, languageId, isPublishing, locale, cancellationToken);
      }
    }

    await ContentRepository.SaveAsync(content, cancellationToken);
  }
  protected virtual async Task ValidateAsync(
    ContentType contentType,
    IReadOnlyDictionary<FieldTypeId, FieldType> fieldTypes,
    ContentId contentId,
    LanguageId? languageId,
    bool isPublishing,
    ContentLocale locale,
    CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = [];
    string propertyName = nameof(locale.FieldValues);

    bool isInvariant = !languageId.HasValue;
    if (isPublishing)
    {
      foreach (FieldDefinition fieldDefinition in contentType.Fields)
      {
        if (fieldDefinition.IsRequired && fieldDefinition.IsInvariant == isInvariant && !locale.FieldValues.ContainsKey(fieldDefinition.Id))
        {
          ValidationFailure failure = new(propertyName, "The specified field is missing.", fieldDefinition.Id)
          {
            CustomState = new { Field = fieldDefinition.DisplayName?.Value ?? fieldDefinition.UniqueName.Value },
            ErrorCode = "RequiredFieldValidator"
          };
          failures.Add(failure);
        }
      }
    }

    Dictionary<Guid, FieldValue> uniqueValues = new(capacity: locale.FieldValues.Count);
    foreach (KeyValuePair<Guid, FieldValue> fieldValue in locale.FieldValues)
    {
      FieldDefinition? fieldDefinition = contentType.TryGetField(fieldValue.Key);
      if (fieldDefinition is null)
      {
        string errorMessage = $"The field is not defined on content type '{contentType.DisplayName?.Value ?? contentType.UniqueName.Value}'.";
        ValidationFailure failure = new(propertyName, errorMessage, fieldValue.Key)
        {
          ErrorCode = "FieldDefinitionValidator"
        };
        failures.Add(failure);
      }
      else if (fieldDefinition.IsInvariant != isInvariant)
      {
        string errorMessage = fieldDefinition.IsInvariant
          ? "The field is defined as invariant, but saved in a localized content."
          : "The field is defined as localized, but saved in an invariant content.";
        ValidationFailure failure = new(propertyName, errorMessage, fieldValue.Key)
        {
          CustomState = new { Field = fieldDefinition.DisplayName?.Value ?? fieldDefinition.UniqueName.Value },
          ErrorCode = "InvariantValidator"
        };
        failures.Add(failure);
      }
      else
      {
        FieldType fieldType = fieldTypes[fieldDefinition.FieldTypeId];
        IFieldValueValidator validator = FieldValueValidatorFactory.Create(fieldType);
        ValidationResult result = await validator.ValidateAsync(fieldValue.Value, propertyName, cancellationToken);
        failures.AddRange(result.Errors);

        if (fieldDefinition.IsUnique)
        {
          uniqueValues[fieldDefinition.Id] = fieldValue.Value;
        }
      }
    }

    if (uniqueValues.Count > 0)
    {
      ContentStatus status = isPublishing ? ContentStatus.Published : ContentStatus.Latest;
      IReadOnlyDictionary<Guid, ContentId> conflicts = await ContentQuerier.FindConflictsAsync(contentType.Id, languageId, status, uniqueValues, contentId, cancellationToken);
      if (conflicts.Count > 0)
      {
        throw new ContentFieldValueConflictException(contentId, languageId, conflicts, propertyName);
      }
    }

    if (failures.Count > 0)
    {
      throw new ValidationException(failures);
    }
  }

  public virtual async Task SaveAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = contentType.Changes.Any(change => change is ContentTypeCreated || change is ContentTypeUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      ContentTypeId? conflictId = await ContentTypeQuerier.FindIdAsync(contentType.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(contentType.Id))
      {
        throw new UniqueNameAlreadyUsedException(contentType, conflictId.Value);
      }
    }

    await ContentTypeRepository.SaveAsync(contentType, cancellationToken);
  }
}
