using Krakenar.Contracts.Contents;
using Krakenar.Core.Contents.Commands;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents;

public class ContentService : IContentService
{
  protected virtual ICommandHandler<CreateContent, ContentDto> CreateContent { get; }
  protected virtual ICommandHandler<SaveContentLocale, ContentDto?> SaveContentLocale { get; }

  public ContentService(
    ICommandHandler<CreateContent, ContentDto> createContent,
    ICommandHandler<SaveContentLocale, ContentDto?> saveContentLocale)
  {
    CreateContent = createContent;
    SaveContentLocale = saveContentLocale;
  }

  public virtual async Task<ContentDto> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken)
  {
    CreateContent command = new(payload);
    return await CreateContent.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    SaveContentLocale command = new(id, payload, language);
    return await SaveContentLocale.HandleAsync(command, cancellationToken);
  }
}
