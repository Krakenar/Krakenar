using Krakenar.Core.Caching;
using Logitar.CQRS;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations.Queries;

public record ReadConfiguration : IQuery<ConfigurationDto>;

public class ReadConfigurationHandler : IQueryHandler<ReadConfiguration, ConfigurationDto>
{
  protected virtual ICacheService CacheService { get; }

  public ReadConfigurationHandler(ICacheService cacheService)
  {
    CacheService = cacheService;
  }

  public virtual Task<ConfigurationDto> HandleAsync(ReadConfiguration _, CancellationToken cancellationToken)
  {
    ConfigurationDto configuration = CacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
    return Task.FromResult(configuration);
  }
}
