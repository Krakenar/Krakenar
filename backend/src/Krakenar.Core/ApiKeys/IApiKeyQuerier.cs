using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Search;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys;

public interface IApiKeyQuerier
{
  Task<ApiKeyDto> ReadAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
  Task<ApiKeyDto?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKeyDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<ApiKeyDto>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
}
