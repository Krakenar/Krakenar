using Krakenar.Core.Caching;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations;

public interface IConfigurationRepository
{
  Task<Configuration?> LoadAsync(CancellationToken cancellationToken = default);
  Task<Configuration?> LoadAsync(long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Configuration configuration, CancellationToken cancellationToken = default);
}

public class ConfigurationRepository : Repository, IConfigurationRepository
{
  protected virtual ICacheService CacheService { get; }
  protected virtual IConfigurationQuerier ConfigurationQuerier { get; }

  public ConfigurationRepository(ICacheService cacheService, IConfigurationQuerier configurationQuerier, IEventStore eventStore) : base(eventStore)
  {
    CacheService = cacheService;
    ConfigurationQuerier = configurationQuerier;
  }

  public virtual async Task<Configuration?> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync(version: null, cancellationToken);
  }
  public virtual async Task<Configuration?> LoadAsync(long? version, CancellationToken cancellationToken)
  {
    ConfigurationId id = new();
    return await LoadAsync<Configuration>(id.StreamId, version, cancellationToken);
  }

  public virtual async Task SaveAsync(Configuration configuration, CancellationToken cancellationToken)
  {
    await base.SaveAsync(configuration, cancellationToken);

    CacheService.Configuration = await ConfigurationQuerier.ReadAsync(configuration, cancellationToken);
  }
}
