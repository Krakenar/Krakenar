using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Search;
using Krakenar.Core.ApiKeys.Commands;
using Krakenar.Core.ApiKeys.Queries;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys;

public class ApiKeyService : IApiKeyService
{
  protected virtual ICommandHandler<AuthenticateApiKey, ApiKeyDto> AuthenticateApiKey { get; }
  protected virtual ICommandHandler<CreateOrReplaceApiKey, CreateOrReplaceApiKeyResult> CreateOrReplaceApiKey { get; }
  protected virtual ICommandHandler<DeleteApiKey, ApiKeyDto?> DeleteApiKey { get; }
  protected virtual IQueryHandler<ReadApiKey, ApiKeyDto?> ReadApiKey { get; }
  protected virtual IQueryHandler<SearchApiKeys, SearchResults<ApiKeyDto>> SearchApiKeys { get; }
  protected virtual ICommandHandler<UpdateApiKey, ApiKeyDto?> UpdateApiKey { get; }

  public ApiKeyService(
    ICommandHandler<AuthenticateApiKey, ApiKeyDto> authenticateApiKey,
    ICommandHandler<CreateOrReplaceApiKey, CreateOrReplaceApiKeyResult> createOrReplaceApiKey,
    ICommandHandler<DeleteApiKey, ApiKeyDto?> deleteApiKey,
    IQueryHandler<ReadApiKey, ApiKeyDto?> readApiKey,
    IQueryHandler<SearchApiKeys, SearchResults<ApiKeyDto>> searchApiKeys,
    ICommandHandler<UpdateApiKey, ApiKeyDto?> updateApiKey)
  {
    AuthenticateApiKey = authenticateApiKey;
    CreateOrReplaceApiKey = createOrReplaceApiKey;
    DeleteApiKey = deleteApiKey;
    ReadApiKey = readApiKey;
    SearchApiKeys = searchApiKeys;
    UpdateApiKey = updateApiKey;
  }

  public virtual async Task<ApiKeyDto> AuthenticateAsync(AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    AuthenticateApiKey command = new(payload);
    return await AuthenticateApiKey.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<CreateOrReplaceApiKeyResult> CreateOrReplaceAsync(CreateOrReplaceApiKeyPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceApiKey command = new(id, payload, version);
    return await CreateOrReplaceApiKey.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ApiKeyDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteApiKey command = new(id);
    return await DeleteApiKey.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ApiKeyDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadApiKey query = new(id);
    return await ReadApiKey.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<ApiKeyDto>> SearchAsync(SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    SearchApiKeys query = new(payload);
    return await SearchApiKeys.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<ApiKeyDto?> UpdateAsync(Guid id, UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    UpdateApiKey command = new(id, payload);
    return await UpdateApiKey.HandleAsync(command, cancellationToken);
  }
}
