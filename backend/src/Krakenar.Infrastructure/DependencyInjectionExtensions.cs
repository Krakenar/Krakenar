using Krakenar.Core.Caching;
using Krakenar.Core.Passwords;
using Krakenar.Core.Tokens;
using Krakenar.Infrastructure.Caching;
using Krakenar.Infrastructure.Passwords;
using Krakenar.Infrastructure.Settings;
using Krakenar.Infrastructure.Tokens;
using Logitar.EventSourcing.Infrastructure;
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
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddSingleton<IPasswordService, PasswordService>()
      .AddSingleton<ISecretService, SecretService>();
  }

  public static CachingSettings InitializeCachingSettings(this IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return CachingSettings.Initialize(configuration);
  }
}
