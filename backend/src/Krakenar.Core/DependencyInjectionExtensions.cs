using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Krakenar.Core.Configurations;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Core.Configurations.Queries;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Commands;
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
using Krakenar.Core.Users.Commands;
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
      .AddKrakenarManagers()
      .AddKrakenarQueries()
      .AddKrakenarRepositories()
      .AddLogitarEventSourcing()
      .AddSingleton<IAddressHelper, AddressHelper>();
  }

  public static IServiceCollection AddKrakenarCommands(this IServiceCollection services)
  {
    return services
      .AddTransient<ICommandHandler<AuthenticateUser, UserDto>, AuthenticateUserHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult>, CreateOrReplaceLanguageHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult>, CreateOrReplaceRealmHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult>, CreateOrReplaceRoleHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceUser, CreateOrReplaceUserResult>, CreateOrReplaceUserHandler>()
      .AddTransient<ICommandHandler<CreateSession, SessionDto>, CreateSessionHandler>()
      .AddTransient<ICommandHandler<DeleteLanguage, LanguageDto?>, DeleteLanguageHandler>()
      .AddTransient<ICommandHandler<DeleteRole, RoleDto?>, DeleteRoleHandler>()
      .AddTransient<ICommandHandler<DeleteUser, UserDto?>, DeleteUserHandler>()
      .AddTransient<ICommandHandler<InitializeConfiguration>, InitializeConfigurationHandler>()
      .AddTransient<ICommandHandler<RemoveUserIdentifier, UserDto?>, RemoveUserIdentifierHandler>()
      .AddTransient<ICommandHandler<RenewSession, SessionDto>, RenewSessionHandler>()
      .AddTransient<ICommandHandler<ReplaceConfiguration, ConfigurationDto>, ReplaceConfigurationHandler>()
      .AddTransient<ICommandHandler<ResetUserPassword, UserDto?>, ResetUserPasswordHandler>()
      .AddTransient<ICommandHandler<SaveUserIdentifier, UserDto?>, SaveUserIdentifierHandler>()
      .AddTransient<ICommandHandler<SetDefaultLanguage, LanguageDto?>, SetDefaultLanguageHandler>()
      .AddTransient<ICommandHandler<SignInSession, SessionDto>, SignInSessionHandler>()
      .AddTransient<ICommandHandler<SignOutSession, SessionDto?>, SignOutSessionHandler>()
      .AddTransient<ICommandHandler<SignOutUser, UserDto?>, SignOutUserHandler>()
      .AddTransient<ICommandHandler<UpdateConfiguration, ConfigurationDto>, UpdateConfigurationHandler>()
      .AddTransient<ICommandHandler<UpdateLanguage, LanguageDto?>, UpdateLanguageHandler>()
      .AddTransient<ICommandHandler<UpdateRealm, RealmDto?>, UpdateRealmHandler>()
      .AddTransient<ICommandHandler<UpdateRole, RoleDto?>, UpdateRoleHandler>()
      .AddTransient<ICommandHandler<UpdateUser, UserDto?>, UpdateUserHandler>();
  }

  public static IServiceCollection AddKrakenarCoreServices(this IServiceCollection services)
  {
    return services
      .AddTransient<IConfigurationService, ConfigurationService>()
      .AddTransient<ILanguageService, LanguageService>()
      .AddTransient<IRealmService, RealmService>()
      .AddTransient<IRoleService, RoleService>()
      .AddTransient<ISessionService, SessionService>()
      .AddTransient<IUserService, UserService>();
  }

  public static IServiceCollection AddKrakenarManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<ILanguageManager, LanguageManager>()
      .AddTransient<IRealmManager, RealmManager>()
      .AddTransient<IRoleManager, RoleManager>()
      .AddTransient<IUserManager, UserManager>();
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
      .AddTransient<IQueryHandler<SearchLanguages, SearchResults<LanguageDto>>, SearchLanguagesHandler>()
      .AddTransient<IQueryHandler<SearchRealms, SearchResults<RealmDto>>, SearchRealmsHandler>()
      .AddTransient<IQueryHandler<SearchRoles, SearchResults<RoleDto>>, SearchRolesHandler>()
      .AddTransient<IQueryHandler<SearchSessions, SearchResults<SessionDto>>, SearchSessionsHandler>()
      .AddTransient<IQueryHandler<SearchUsers, SearchResults<UserDto>>, SearchUsersHandler>();
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
