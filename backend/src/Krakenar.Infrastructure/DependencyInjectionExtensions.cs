using Krakenar.Core.Caching;
using Krakenar.Infrastructure.Caching;
using Krakenar.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarInfrastructure(this IServiceCollection services)
  {
    return services
      .AddMemoryCache()
      .AddSingleton(InitializeCachingSettings)
      .AddSingleton<ICacheService, CacheService>();
  }

  public static CachingSettings InitializeCachingSettings(this IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return CachingSettings.Initialize(configuration);
  }
}
