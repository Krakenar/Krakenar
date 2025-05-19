using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Search;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys.Queries;

public record SearchApiKeys(SearchApiKeysPayload Payload) : IQuery<SearchResults<ApiKeyDto>>;

public class SearchApiKeysHandler : IQueryHandler<SearchApiKeys, SearchResults<ApiKeyDto>>
{
  protected virtual IApiKeyQuerier ApiKeyQuerier { get; }

  public SearchApiKeysHandler(IApiKeyQuerier apiKeyQuerier)
  {
    ApiKeyQuerier = apiKeyQuerier;
  }

  public virtual async Task<SearchResults<ApiKeyDto>> HandleAsync(SearchApiKeys query, CancellationToken cancellationToken)
  {
    return await ApiKeyQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
