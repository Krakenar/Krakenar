using Krakenar.Core.Localization;
using Logitar.EventSourcing;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents.Commands;

public record UnpublishContent : ICommand<ContentDto?>
{
  public Guid ContentId { get; }
  public string? Language { get; }
  public bool All { get; }

  public UnpublishContent(Guid contentId)
  {
    ContentId = contentId;
    All = true;
  }

  public UnpublishContent(Guid contentId, string? language)
  {
    ContentId = contentId;
    Language = language;
  }
}

public class UnpublishContentHandler : ICommandHandler<UnpublishContent, ContentDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentManager ContentManager { get; }
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IContentRepository ContentRepository { get; }
  protected virtual ILanguageManager LanguageManager { get; }

  public UnpublishContentHandler(
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

  public virtual async Task<ContentDto?> HandleAsync(UnpublishContent command, CancellationToken cancellationToken)
  {
    ContentId contentId = new(command.ContentId, ApplicationContext.RealmId);
    Content? content = await ContentRepository.LoadAsync(contentId, cancellationToken);
    if (content is null)
    {
      return null;
    }

    ActorId? actorId = ApplicationContext.ActorId;
    if (command.All)
    {
      content.Unpublish(actorId);
    }
    else if (!string.IsNullOrWhiteSpace(command.Language))
    {
      Language language = await LanguageManager.FindAsync(command.Language, nameof(command.Language), cancellationToken);
      if (!content.UnpublishLocale(language, actorId))
      {
        return null;
      }
    }
    else
    {
      content.UnpublishInvariant(actorId);
    }

    await ContentManager.SaveAsync(content, contentType: null, cancellationToken);

    return await ContentQuerier.ReadAsync(content, cancellationToken);
  }
}
