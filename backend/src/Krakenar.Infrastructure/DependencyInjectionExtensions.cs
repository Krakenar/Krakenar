using Krakenar.Core.Caching;
using Krakenar.Core.Passwords;
using Krakenar.Core.Tokens;
using Krakenar.Infrastructure.Caching;
using Krakenar.Infrastructure.Converters;
using Krakenar.Infrastructure.Passwords;
using Krakenar.Infrastructure.Passwords.Pbkdf2;
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
      .AddKrakenarPasswordStrategies()
      .AddMemoryCache()
      .AddSingleton(serviceProvider => CachingSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => EncryptionSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => Pbkdf2Settings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddSingleton<IPasswordService, PasswordService>()
      .AddSingleton<ISecretService, SecretService>()
      .AddSingleton<PasswordConverter>()
      .AddScoped<IEventBus, EventBus>();
  }

  public static IServiceCollection AddKrakenarPasswordStrategies(this IServiceCollection services)
  {
    return services.AddSingleton<IPasswordStrategy, Pbkdf2Strategy>();
  }
}
