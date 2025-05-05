using Logitar.EventSourcing;

namespace Krakenar.Core.ApiKeys;

public interface IApiKeyRepository
{
  Task<ApiKey?> LoadAsync(ApiKeyId id, CancellationToken cancellationToken = default);
  Task<ApiKey?> LoadAsync(ApiKeyId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(ApiKey apiKey, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ApiKey> apiKeys, CancellationToken cancellationToken = default);
}

public class ApiKeyRepository : Repository, IApiKeyRepository
{
  public ApiKeyRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<ApiKey?> LoadAsync(ApiKeyId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<ApiKey?> LoadAsync(ApiKeyId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<ApiKey>(id.StreamId, version, cancellationToken);
  }
  public virtual async Task<IReadOnlyCollection<ApiKey>> LoadAsync(IEnumerable<ApiKeyId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync<ApiKey>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public virtual async Task SaveAsync(ApiKey role, CancellationToken cancellationToken)
  {
    await base.SaveAsync(role, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<ApiKey> roles, CancellationToken cancellationToken)
  {
    await base.SaveAsync(roles, cancellationToken);
  }
}
