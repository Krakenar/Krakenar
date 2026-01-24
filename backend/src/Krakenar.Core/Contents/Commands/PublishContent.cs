using Krakenar.Core.Localization;
using Logitar.CQRS;
using Logitar.EventSourcing;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents.Commands;

public record PublishContent : ICommand<ContentDto?>
{
  public Guid ContentId { get; }
  public string? Language { get; }
  public bool All { get; }

  public PublishContent(Guid contentId)
  {
    ContentId = contentId;
    All = true;
  }

  public PublishContent(Guid contentId, string? language)
  {
    ContentId = contentId;
    Language = language;
  }
}

public class PublishContentHandler : ICommandHandler<PublishContent, ContentDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentManager ContentManager { get; }
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IContentRepository ContentRepository { get; }
  protected virtual ILanguageManager LanguageManager { get; }

  public PublishContentHandler(
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

  public virtual async Task<ContentDto?> HandleAsync(PublishContent command, CancellationToken cancellationToken)
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
      content.Publish(actorId);
    }
    else if (!string.IsNullOrWhiteSpace(command.Language))
    {
      Language language = await LanguageManager.FindAsync(command.Language, nameof(command.Language), cancellationToken);
      if (!content.PublishLocale(language, actorId))
      {
        return null;
      }
    }
    else
    {
      content.PublishInvariant(actorId);
    }

    await ContentManager.SaveAsync(content, contentType: null, cancellationToken);

    return await ContentQuerier.ReadAsync(content, cancellationToken);
  }
}
