using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Contents;

public interface IContentService
{
  Task<Content> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken = default);
  Task<Content?> DeleteAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<Content?> PublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Content?> PublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<Content?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Content?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language = null, CancellationToken cancellationToken = default);
  Task<SearchResults<ContentLocale>> SearchLocalesAsync(SearchContentLocalesPayload payload, CancellationToken cancellationToken = default);
  Task<Content?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Content?> UnpublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<Content?> UpdateLocaleAsync(Guid id, UpdateContentLocalePayload payload, string? language = null, CancellationToken cancellationToken = default);
}
