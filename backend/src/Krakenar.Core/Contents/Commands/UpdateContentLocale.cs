using FluentValidation;
using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Contents.Validators;
using Krakenar.Core.Localization;
using ContentDto = Krakenar.Contracts.Contents.Content;
using FieldDefinition = Krakenar.Core.Fields.FieldDefinition;
using FieldValue = Krakenar.Core.Fields.FieldValue;

namespace Krakenar.Core.Contents.Commands;

public record UpdateContentLocale(Guid Id, UpdateContentLocalePayload Payload, string? Language) : ICommand<ContentDto?>;

/// <exception cref="ContentUniqueNameAlreadyUsedException"></exception>
/// <exception cref="LanguageNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateContentLocaleHandler : ICommandHandler<UpdateContentLocale, ContentDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentManager ContentManager { get; }
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IContentRepository ContentRepository { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }
  protected virtual ILanguageManager LanguageManager { get; }

  public UpdateContentLocaleHandler(
    IApplicationContext applicationContext,
    IContentManager contentManager,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    IContentTypeRepository contentTypeRepository,
    ILanguageManager languageManager)
  {
    ApplicationContext = applicationContext;
    ContentManager = contentManager;
    ContentQuerier = contentQuerier;
    ContentRepository = contentRepository;
    ContentTypeRepository = contentTypeRepository;
    LanguageManager = languageManager;
  }

  public virtual async Task<ContentDto?> HandleAsync(UpdateContentLocale command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    UpdateContentLocalePayload payload = command.Payload;
    new UpdateContentLocaleValidator(uniqueNameSettings).ValidateAndThrow(payload);

    ContentId contentId = new(command.Id, ApplicationContext.RealmId);
    Content? content = await ContentRepository.LoadAsync(contentId, cancellationToken);
    if (content is null)
    {
      return null;
    }

    ContentType contentType = await ContentTypeRepository.LoadAsync(content.ContentTypeId, cancellationToken)
      ?? throw new InvalidOperationException($"The content type 'Id={content.ContentTypeId}' was not loaded.");

    Language? language = null;
    ContentLocale invariantOrLocale = content.Invariant;
    if (!string.IsNullOrWhiteSpace(command.Language))
    {
      if (contentType.IsInvariant)
      {
        string errorMessage = $"'{nameof(command.Language)}' must be null. The content type is invariant.";
        ValidationFailure failure = new(nameof(command.Language), errorMessage, command.Language)
        {
          ErrorCode = "InvariantValidator"
        };
        throw new ValidationException([failure]);
      }

      language = await LanguageManager.FindAsync(command.Language, nameof(command.Language), cancellationToken);

      invariantOrLocale = content.TryGetLocale(language)!;
      if (invariantOrLocale is null)
      {
        return null;
      }
    }

    UniqueName uniqueName = string.IsNullOrWhiteSpace(payload.UniqueName) ? invariantOrLocale.UniqueName : new(uniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = payload.DisplayName is null ? invariantOrLocale.DisplayName : DisplayName.TryCreate(payload.DisplayName.Value);
    Description? description = payload.Description is null ? invariantOrLocale.Description : Description.TryCreate(payload.Description.Value);

    Dictionary<Guid, FieldValue> fieldValues = new(invariantOrLocale.FieldValues);
    foreach (FieldValuePayload fieldValue in payload.FieldValues)
    {
      FieldDefinition? field = contentType.ResolveField(fieldValue.Field);
      if (field is null)
      {
        // TODO(fpion): implement
      }
      else if (string.IsNullOrWhiteSpace(fieldValue.Value))
      {
        fieldValues.Remove(field.Id);
      }
      else
      {
        fieldValues[field.Id] = new FieldValue(fieldValue.Value);
      }
    }

    invariantOrLocale = new(uniqueName, displayName, description, fieldValues.AsReadOnly());
    if (language is null)
    {
      content.SetInvariant(invariantOrLocale, ApplicationContext.ActorId);
    }
    else
    {
      content.SetLocale(language, invariantOrLocale, ApplicationContext.ActorId);
    }

    await ContentManager.SaveAsync(content, cancellationToken);

    return await ContentQuerier.ReadAsync(content, cancellationToken);
  }
}
