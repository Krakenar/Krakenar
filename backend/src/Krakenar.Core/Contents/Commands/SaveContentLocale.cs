using FluentValidation;
using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Contents.Validators;
using Krakenar.Core.Fields;
using Krakenar.Core.Localization;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents.Commands;

public record SaveContentLocale(Guid Id, SaveContentLocalePayload Payload, string? Language) : ICommand<ContentDto?>;

/// <exception cref="ContentUniqueNameAlreadyUsedException"></exception>
/// <exception cref="LanguageNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class SaveContentLocaleHandler : ICommandHandler<SaveContentLocale, ContentDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentManager ContentManager { get; }
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IContentRepository ContentRepository { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }
  protected virtual ILanguageManager LanguageManager { get; }

  public SaveContentLocaleHandler(
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

  public virtual async Task<ContentDto?> HandleAsync(SaveContentLocale command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    SaveContentLocalePayload payload = command.Payload;
    new SaveContentLocaleValidator(uniqueNameSettings).ValidateAndThrow(payload);

    ContentId contentId = new(command.Id, ApplicationContext.RealmId);
    Content? content = await ContentRepository.LoadAsync(contentId, cancellationToken);
    if (content is null)
    {
      return null;
    }

    Language? language = null;
    if (!string.IsNullOrWhiteSpace(command.Language))
    {
      ContentType contentType = await ContentTypeRepository.LoadAsync(content.ContentTypeId, cancellationToken)
        ?? throw new InvalidOperationException($"The content type 'Id={content.ContentTypeId}' was not loaded.");
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
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);
    ContentLocale invariantOrLocale = new(uniqueName, displayName, description, new Dictionary<Guid, FieldValue>().AsReadOnly()); // TODO(fpion): implement
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
