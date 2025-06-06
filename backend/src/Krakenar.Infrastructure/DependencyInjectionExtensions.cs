﻿using Krakenar.Core.Caching;
using Krakenar.Core.Encryption;
using Krakenar.Core.Messages;
using Krakenar.Core.Passwords;
using Krakenar.Core.Tokens;
using Krakenar.Infrastructure.Caching;
using Krakenar.Infrastructure.Converters;
using Krakenar.Infrastructure.Encryption;
using Krakenar.Infrastructure.Messages;
using Krakenar.Infrastructure.Messages.Providers;
using Krakenar.Infrastructure.Messages.Providers.SendGrid;
using Krakenar.Infrastructure.Messages.Providers.Twilio;
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
      .AddKrakenarSenderStrategies()
      .AddMemoryCache()
      .AddSingleton(serviceProvider => CachingSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => EncryptionSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => Pbkdf2Settings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEncryptionManager, EncryptionManager>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddSingleton<IMessageManager, MessageManager>()
      .AddSingleton<IPasswordManager, PasswordManager>()
      .AddSingleton<ISecretManager, SecretManager>()
      .AddSingleton<PasswordConverter>()
      .AddScoped<IEventBus, EventBus>()
      .AddScoped<ITokenManager, JsonWebTokenManager>();
  }

  public static IServiceCollection AddKrakenarPasswordStrategies(this IServiceCollection services)
  {
    return services.AddSingleton<IPasswordStrategy, Pbkdf2Strategy>();
  }

  public static IServiceCollection AddKrakenarSenderStrategies(this IServiceCollection services)
  {
    return services
      .AddSingleton<IProviderStrategy, SendGridStrategy>()
      .AddSingleton<IProviderStrategy, TwilioStrategy>();
  }
}
