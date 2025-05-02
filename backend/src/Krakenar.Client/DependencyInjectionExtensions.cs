using Krakenar.Client.Configurations;
using Krakenar.Client.Dictionaries;
using Krakenar.Client.Localization;
using Krakenar.Client.Realms;
using Krakenar.Client.Roles;
using Krakenar.Client.Sessions;
using Krakenar.Client.Users;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.Client;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarClient(this IServiceCollection services, IConfiguration configuration)
  {
    KrakenarSettings settings = KrakenarSettings.Initialize(configuration);
    return services.AddKrakenarClient(settings);
  }
  public static IServiceCollection AddKrakenarClient(this IServiceCollection services, IKrakenarSettings settings)
  {
    return services
      .AddHttpClient()
      .AddSingleton(settings)
      .AddSingleton<IConfigurationService, ConfigurationClient>()
      .AddSingleton<IDictionaryService, DictionaryClient>()
      .AddSingleton<ILanguageService, LanguageClient>()
      .AddSingleton<IRealmService, RealmClient>()
      .AddSingleton<IRoleService, RoleClient>()
      .AddSingleton<ISessionService, SessionClient>()
      .AddSingleton<IUserService, UserClient>();
  }
}
