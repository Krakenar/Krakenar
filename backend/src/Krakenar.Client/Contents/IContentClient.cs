using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Contents;

public interface IContentClient : IContentService
{
  Task<Content> CreateAsync(CreateContentPayload payload, RequestContext? context);
  Task<Content?> DeleteAsync(Guid id, string? language, RequestContext? context);
  Task<Content?> PublishAllAsync(Guid id, RequestContext? context);
  Task<Content?> PublishAsync(Guid id, string? language, RequestContext? context);
  Task<Content?> ReadAsync(Guid id, RequestContext? context);
  Task<Content?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language, RequestContext? context);
  Task<SearchResults<ContentLocale>> SearchLocalesAsync(SearchContentLocalesPayload payload, RequestContext? context);
  Task<Content?> UnpublishAllAsync(Guid id, RequestContext? context);
  Task<Content?> UnpublishAsync(Guid id, string? language, RequestContext? context);
  Task<Content?> UpdateLocaleAsync(Guid id, UpdateContentLocalePayload payload, string? language, RequestContext? context);
}
