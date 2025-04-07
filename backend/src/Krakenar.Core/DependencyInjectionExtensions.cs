using Krakenar.Core.Configurations;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Core.Configurations.Queries;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Krakenar.Core.Realms.Commands;
using Krakenar.Core.Realms.Queries;
using Krakenar.Core.Roles;
using Krakenar.Core.Roles.Commands;
using Krakenar.Core.Roles.Queries;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarCore(this IServiceCollection services)
  {
    return services
      .AddKrakenarCommands()
      .AddKrakenarCoreServices()
      .AddKrakenarQueries()
      .AddKrakenarRepositories()
      .AddLogitarEventSourcing()
      .AddSingleton<IAddressHelper, AddressHelper>();
  }

  public static IServiceCollection AddKrakenarCommands(this IServiceCollection services)
  {
    return services
      .AddTransient<ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult>, CreateOrReplaceRealmHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult>, CreateOrReplaceRoleHandler>()
      .AddTransient<ICommandHandler<InitializeConfiguration>, InitializeConfigurationHandler>();
  }

  public static IServiceCollection AddKrakenarCoreServices(this IServiceCollection services)
  {
    return services
      .AddTransient<ILanguageService, LanguageService>()
      .AddTransient<IRealmService, RealmService>()
      .AddTransient<IRoleService, RoleService>()
      .AddTransient<IUserService, UserService>();
  }

  public static IServiceCollection AddKrakenarQueries(this IServiceCollection services)
  {
    return services
      .AddTransient<IQueryHandler<ReadConfiguration, ConfigurationDto>, ReadConfigurationHandler>()
      .AddTransient<IQueryHandler<ReadRealm, RealmDto?>, ReadRealmHandler>()
      .AddTransient<IQueryHandler<ReadRole, RoleDto?>, ReadRoleHandler>();
  }

  public static IServiceCollection AddKrakenarRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationRepository, ConfigurationRepository>()
      .AddTransient<ILanguageRepository, LanguageRepository>()
      .AddTransient<IRealmRepository, RealmRepository>()
      .AddTransient<IRoleRepository, RoleRepository>()
      .AddTransient<IUserRepository, UserRepository>();
  }
}
