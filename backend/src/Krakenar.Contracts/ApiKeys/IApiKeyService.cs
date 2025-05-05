using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.ApiKeys;

public interface IApiKeyService
{
  Task<ApiKey> AuthenticateAsync(AuthenticateApiKeyPayload payload, CancellationToken cancellationToken = default);
  Task<CreateOrReplaceApiKeyResult> CreateOrReplaceAsync(CreateOrReplaceApiKeyPayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<ApiKey?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ApiKey?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<ApiKey>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
  Task<ApiKey?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken = default);
}
