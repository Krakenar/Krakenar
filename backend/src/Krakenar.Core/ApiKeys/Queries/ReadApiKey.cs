using Krakenar.Contracts;
using Logitar.CQRS;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys.Queries;

public record ReadApiKey(Guid Id) : IQuery<ApiKeyDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadApiKeyHandler : IQueryHandler<ReadApiKey, ApiKeyDto?>
{
  protected virtual IApiKeyQuerier ApiKeyQuerier { get; }

  public ReadApiKeyHandler(IApiKeyQuerier apiKeyQuerier)
  {
    ApiKeyQuerier = apiKeyQuerier;
  }

  public virtual async Task<ApiKeyDto?> HandleAsync(ReadApiKey query, CancellationToken cancellationToken)
  {
    return await ApiKeyQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
