using Krakenar.Core.Localization;
using Logitar.CQRS;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents.Commands;

public record DeleteContent(Guid Id, string? Language) : ICommand<ContentDto?>;

/// <exception cref="LanguageNotFoundException"></exception>
public class DeleteContentHandler : ICommandHandler<DeleteContent, ContentDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IContentRepository ContentRepository { get; }
  protected virtual ILanguageManager LanguageManager { get; }

  public DeleteContentHandler(
    IApplicationContext applicationContext,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    ILanguageManager languageManager)
  {
    ApplicationContext = applicationContext;
    ContentQuerier = contentQuerier;
    ContentRepository = contentRepository;
    LanguageManager = languageManager;
  }

  public virtual async Task<ContentDto?> HandleAsync(DeleteContent command, CancellationToken cancellationToken)
  {
    ContentId contentId = new(command.Id, ApplicationContext.RealmId);
    Content? content = await ContentRepository.LoadAsync(contentId, cancellationToken);
    if (content is null)
    {
      return null;
    }

    ContentDto? dto = null;
    if (string.IsNullOrWhiteSpace(command.Language))
    {
      dto = await ContentQuerier.ReadAsync(content, cancellationToken);

      content.Delete(ApplicationContext.ActorId);
    }
    else
    {
      Language language = await LanguageManager.FindAsync(command.Language, nameof(command.Language), cancellationToken);
      if (!content.RemoveLocale(language, ApplicationContext.ActorId))
      {
        return null;
      }
    }

    await ContentRepository.SaveAsync(content, cancellationToken);

    return dto ?? await ContentQuerier.ReadAsync(content, cancellationToken);
  }
}
