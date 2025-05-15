using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Contents;

public interface IContentTypeService
{
  Task<CreateOrReplaceContentTypeResult> CreateOrReplaceAsync(CreateOrReplaceContentTypePayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<ContentType?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ContentType?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<ContentType>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken = default);
  Task<ContentType?> UpdateAsync(Guid id, UpdateContentTypePayload payload, CancellationToken cancellationToken = default);
}
