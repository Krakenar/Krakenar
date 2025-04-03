using Krakenar.Contracts.Configurations;
using Krakenar.Core.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Krakenar.Infrastructure.Caching;

public class CacheService : ICacheService
{
  private const string ConfigurationKey = "Configuration";

  private readonly IMemoryCache _memoryCache;

  public CacheService(IMemoryCache memoryCache)
  {
    _memoryCache = memoryCache;
  }

  public virtual ConfigurationModel? Configuration
  {
    get => _memoryCache.TryGetValue(ConfigurationKey, out ConfigurationModel? configuration) ? configuration : null;
    set => _memoryCache.Set(ConfigurationKey, value);
  }
}
