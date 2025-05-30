using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Search;
using Krakenar.Core.ApiKeys.Commands;
using Krakenar.Core.ApiKeys.Queries;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys;

public class ApiKeyService : IApiKeyService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public ApiKeyService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<ApiKeyDto> AuthenticateAsync(AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    AuthenticateApiKey command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<CreateOrReplaceApiKeyResult> CreateOrReplaceAsync(CreateOrReplaceApiKeyPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceApiKey command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ApiKeyDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteApiKey command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ApiKeyDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadApiKey query = new(id);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<ApiKeyDto>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    SearchApiKeys query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<ApiKeyDto?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    UpdateApiKey command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
