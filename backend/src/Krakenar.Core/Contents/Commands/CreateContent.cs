using FluentValidation;
using FluentValidation.Results;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Contents.Validators;
using Krakenar.Core.Localization;
using Logitar.EventSourcing;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents.Commands;

public record CreateContent(CreateContentPayload Payload) : ICommand<ContentDto>;

/// <exception cref="ContentTypeNotFoundException"></exception>
/// <exception cref="ContentUniqueNameAlreadyUsedException"></exception>
/// <exception cref="IdAlreadyUsedException{T}"></exception>
/// <exception cref="LanguageNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateContentHandler : ICommandHandler<CreateContent, ContentDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentManager ContentManager { get; }
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IContentRepository ContentRepository { get; }
  protected virtual ILanguageManager LanguageManager { get; }

  public CreateContentHandler(
    IApplicationContext applicationContext,
    IContentManager contentManager,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    ILanguageManager languageManager)
  {
    ApplicationContext = applicationContext;
    ContentManager = contentManager;
    ContentQuerier = contentQuerier;
    ContentRepository = contentRepository;
    LanguageManager = languageManager;
  }

  public virtual async Task<ContentDto> HandleAsync(CreateContent command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    CreateContentPayload payload = command.Payload;
    new CreateContentValidator(ApplicationContext.UniqueNameSettings).ValidateAndThrow(payload);

    ContentType contentType = await ContentManager.FindAsync(payload.ContentType, nameof(payload.ContentType), cancellationToken);

    Language? language = null;
    string? errorMessage = null;
    if (string.IsNullOrWhiteSpace(payload.Language))
    {
      if (!contentType.IsInvariant)
      {
        errorMessage = $"'{nameof(payload.Language)}' cannot be null. The content type is not invariant.";
      }
    }
    else if (contentType.IsInvariant)
    {
      errorMessage = $"'{nameof(payload.Language)}' must be null. The content type is invariant.";
    }
    else
    {
      language = await LanguageManager.FindAsync(payload.Language, nameof(payload.Language), cancellationToken);
    }
    if (errorMessage is not null)
    {
      ValidationFailure failure = new(nameof(payload.Language), errorMessage, payload.Language)
      {
        ErrorCode = "InvariantValidator"
      };
      throw new ValidationException([failure]);
    }

    ContentId contentId = ContentId.NewId(ApplicationContext.RealmId);
    Content? content;
    if (payload.Id.HasValue)
    {
      contentId = new(payload.Id.Value, contentId.RealmId);
      content = await ContentRepository.LoadAsync(contentId, cancellationToken);
      if (content is not null)
      {
        throw new IdAlreadyUsedException<Content>(content.RealmId, content.EntityId, nameof(payload.Id));
      }
    }

    ActorId? actorId = ApplicationContext.ActorId;

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    Description? description = Description.TryCreate(payload.Description);
    ContentLocale invariant = new(uniqueName, displayName, description);

    content = new Content(contentType, invariant, actorId, contentId);

    if (language is not null)
    {
      content.SetLocale(language, invariant, actorId);
    }

    await ContentManager.SaveAsync(content, cancellationToken);

    return await ContentQuerier.ReadAsync(content, cancellationToken);
  }
}
