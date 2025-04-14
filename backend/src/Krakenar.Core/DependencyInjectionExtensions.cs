using Krakenar.Contracts.Search;
using Krakenar.Core.Configurations;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Core.Configurations.Queries;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Queries;
using Krakenar.Core.Realms;
using Krakenar.Core.Realms.Commands;
using Krakenar.Core.Realms.Queries;
using Krakenar.Core.Roles;
using Krakenar.Core.Roles.Commands;
using Krakenar.Core.Roles.Queries;
using Krakenar.Core.Sessions;
using Krakenar.Core.Sessions.Commands;
using Krakenar.Core.Sessions.Queries;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Queries;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;
using LanguageDto = Krakenar.Contracts.Localization.Language;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using RoleDto = Krakenar.Contracts.Roles.Role;
using SessionDto = Krakenar.Contracts.Sessions.Session;
using UserDto = Krakenar.Contracts.Users.User;

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
      .AddTransient<ICommandHandler<CreateSession, SessionDto>, CreateSessionHandler>()
      .AddTransient<ICommandHandler<InitializeConfiguration>, InitializeConfigurationHandler>()
      .AddTransient<ICommandHandler<RenewSession, SessionDto>, RenewSessionHandler>()
      .AddTransient<ICommandHandler<SignInSession, SessionDto>, SignInSessionHandler>()
      .AddTransient<ICommandHandler<SignOutSession, SessionDto?>, SignOutSessionHandler>()
      .AddTransient<ICommandHandler<UpdateRealm, RealmDto?>, UpdateRealmHandler>()
      .AddTransient<ICommandHandler<UpdateRole, RoleDto?>, UpdateRoleHandler>();
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
      .AddTransient<IQueryHandler<ReadLanguage, LanguageDto?>, ReadLanguageHandler>()
      .AddTransient<IQueryHandler<ReadRealm, RealmDto?>, ReadRealmHandler>()
      .AddTransient<IQueryHandler<ReadRole, RoleDto?>, ReadRoleHandler>()
      .AddTransient<IQueryHandler<ReadSession, SessionDto?>, ReadSessionHandler>()
      .AddTransient<IQueryHandler<ReadUser, UserDto?>, ReadUserHandler>()
      .AddTransient<IQueryHandler<SearchRoles, SearchResults<RoleDto>>, SearchRolesHandler>()
      .AddTransient<IQueryHandler<SearchSessions, SearchResults<SessionDto>>, SearchSessionsHandler>();
  }

  public static IServiceCollection AddKrakenarRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationRepository, ConfigurationRepository>()
      .AddTransient<ILanguageRepository, LanguageRepository>()
      .AddTransient<IRealmRepository, RealmRepository>()
      .AddTransient<IRoleRepository, RoleRepository>()
      .AddTransient<ISessionRepository, SessionRepository>()
      .AddTransient<IUserRepository, UserRepository>();
  }
}
