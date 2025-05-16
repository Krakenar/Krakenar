using Krakenar.Contracts.Contents;
using Krakenar.Core.Contents.Commands;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents;

public class ContentService : IContentService
{
  protected virtual ICommandHandler<CreateContent, ContentDto> CreateContent { get; }
  protected virtual ICommandHandler<DeleteContent, ContentDto?> DeleteContent { get; }
  protected virtual ICommandHandler<SaveContentLocale, ContentDto?> SaveContentLocale { get; }
  protected virtual ICommandHandler<UpdateContentLocale, ContentDto?> UpdateContentLocale { get; }

  public ContentService(
    ICommandHandler<CreateContent, ContentDto> createContent,
    ICommandHandler<DeleteContent, ContentDto?> deleteContent,
    ICommandHandler<SaveContentLocale, ContentDto?> saveContentLocale,
    ICommandHandler<UpdateContentLocale, ContentDto?> updateContentLocale)
  {
    CreateContent = createContent;
    DeleteContent = deleteContent;
    SaveContentLocale = saveContentLocale;
    UpdateContentLocale = updateContentLocale;
  }

  public virtual async Task<ContentDto> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken)
  {
    CreateContent command = new(payload);
    return await CreateContent.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> DeleteAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    DeleteContent command = new(id, language);
    return await DeleteContent.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    SaveContentLocale command = new(id, payload, language);
    return await SaveContentLocale.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> UpdateLocaleAsync(Guid id, UpdateContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    UpdateContentLocale command = new(id, payload, language);
    return await UpdateContentLocale.HandleAsync(command, cancellationToken);
  }
}
