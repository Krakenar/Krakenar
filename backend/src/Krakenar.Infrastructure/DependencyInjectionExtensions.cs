using Krakenar.Core.Caching;
using Krakenar.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarInfrastructure(this IServiceCollection services)
  {
    return services
      .AddMemoryCache()
      .AddSingleton<ICacheService, CacheService>();
  }
}
