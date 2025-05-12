using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Search;
using Krakenar.Core.Roles;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys;

public interface IApiKeyQuerier
{
  Task<IReadOnlyCollection<ApiKeyId>> FindIdsAsync(RoleId roleId, CancellationToken cancellationToken = default);

  Task<ApiKeyDto> ReadAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
  Task<ApiKeyDto?> ReadAsync(ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKeyDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<ApiKeyDto>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken = default);
}
