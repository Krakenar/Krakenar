using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core.Contents.Commands;
using Krakenar.Core.Contents.Queries;
using ContentDto = Krakenar.Contracts.Contents.Content;
using ContentLocaleDto = Krakenar.Contracts.Contents.ContentLocale;

namespace Krakenar.Core.Contents;

public class ContentService : IContentService
{
  protected virtual ICommandHandler<CreateContent, ContentDto> CreateContent { get; }
  protected virtual ICommandHandler<DeleteContent, ContentDto?> DeleteContent { get; }
  protected virtual ICommandHandler<PublishContent, ContentDto?> PublishContent { get; }
  protected virtual IQueryHandler<ReadContent, ContentDto?> ReadContent { get; }
  protected virtual ICommandHandler<SaveContentLocale, ContentDto?> SaveContentLocale { get; }
  protected virtual IQueryHandler<SearchContentLocales, SearchResults<ContentLocaleDto>> SearchContentLocales { get; }
  protected virtual ICommandHandler<UnpublishContent, ContentDto?> UnpublishContent { get; }
  protected virtual ICommandHandler<UpdateContentLocale, ContentDto?> UpdateContentLocale { get; }

  public ContentService(
    ICommandHandler<CreateContent, ContentDto> createContent,
    ICommandHandler<DeleteContent, ContentDto?> deleteContent,
    ICommandHandler<PublishContent, ContentDto?> publishContent,
    IQueryHandler<ReadContent, ContentDto?> readContent,
    ICommandHandler<SaveContentLocale, ContentDto?> saveContentLocale,
    IQueryHandler<SearchContentLocales, SearchResults<ContentLocaleDto>> searchContentLocales,
    ICommandHandler<UnpublishContent, ContentDto?> unpublishContent,
    ICommandHandler<UpdateContentLocale, ContentDto?> updateContentLocale)
  {
    CreateContent = createContent;
    DeleteContent = deleteContent;
    PublishContent = publishContent;
    ReadContent = readContent;
    SaveContentLocale = saveContentLocale;
    SearchContentLocales = searchContentLocales;
    UnpublishContent = unpublishContent;
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

  public virtual async Task<ContentDto?> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishContent command = new(id);
    return await PublishContent.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    PublishContent command = new(id, language);
    return await PublishContent.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadContent query = new(id);
    return await ReadContent.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<ContentDto?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    SaveContentLocale command = new(id, payload, language);
    return await SaveContentLocale.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<SearchResults<ContentLocaleDto>> SearchLocalesAsync(SearchContentLocalesPayload payload, CancellationToken cancellationToken)
  {
    SearchContentLocales query = new(payload);
    return await SearchContentLocales.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<ContentDto?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    UnpublishContent command = new(id);
    return await UnpublishContent.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    UnpublishContent command = new(id, language);
    return await UnpublishContent.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> UpdateLocaleAsync(Guid id, UpdateContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    UpdateContentLocale command = new(id, payload, language);
    return await UpdateContentLocale.HandleAsync(command, cancellationToken);
  }
}
