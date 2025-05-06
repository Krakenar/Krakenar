using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Passwords;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Templates;
using Krakenar.Contracts.Tokens;
using Krakenar.Contracts.Users;
using Krakenar.Core.ApiKeys;
using Krakenar.Core.ApiKeys.Commands;
using Krakenar.Core.ApiKeys.Queries;
using Krakenar.Core.Configurations;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Core.Configurations.Queries;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Dictionaries.Commands;
using Krakenar.Core.Dictionaries.Queries;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Commands;
using Krakenar.Core.Localization.Queries;
using Krakenar.Core.Passwords;
using Krakenar.Core.Passwords.Commands;
using Krakenar.Core.Passwords.Queries;
using Krakenar.Core.Realms;
using Krakenar.Core.Realms.Commands;
using Krakenar.Core.Realms.Queries;
using Krakenar.Core.Roles;
using Krakenar.Core.Roles.Commands;
using Krakenar.Core.Roles.Queries;
using Krakenar.Core.Senders;
using Krakenar.Core.Senders.Commands;
using Krakenar.Core.Sessions;
using Krakenar.Core.Sessions.Commands;
using Krakenar.Core.Sessions.Queries;
using Krakenar.Core.Templates;
using Krakenar.Core.Templates.Commands;
using Krakenar.Core.Templates.Queries;
using Krakenar.Core.Tokens;
using Krakenar.Core.Tokens.Commands;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Commands;
using Krakenar.Core.Users.Queries;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;
using LanguageDto = Krakenar.Contracts.Localization.Language;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using RoleDto = Krakenar.Contracts.Roles.Role;
using SessionDto = Krakenar.Contracts.Sessions.Session;
using TemplateDto = Krakenar.Contracts.Templates.Template;
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
      .AddTransient<ICommandHandler<AuthenticateApiKey, ApiKeyDto>, AuthenticateApiKeyHandler>()
      .AddTransient<ICommandHandler<AuthenticateUser, UserDto>, AuthenticateUserHandler>()
      .AddTransient<ICommandHandler<CreateOneTimePassword, OneTimePasswordDto>, CreateOneTimePasswordHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceApiKey, CreateOrReplaceApiKeyResult>, CreateOrReplaceApiKeyHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceDictionary, CreateOrReplaceDictionaryResult>, CreateOrReplaceDictionaryHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult>, CreateOrReplaceLanguageHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult>, CreateOrReplaceRealmHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult>, CreateOrReplaceRoleHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceSender, CreateOrReplaceSenderResult>, CreateOrReplaceSenderHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceTemplate, CreateOrReplaceTemplateResult>, CreateOrReplaceTemplateHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceUser, CreateOrReplaceUserResult>, CreateOrReplaceUserHandler>()
      .AddTransient<ICommandHandler<CreateSession, SessionDto>, CreateSessionHandler>()
      .AddTransient<ICommandHandler<CreateToken, CreatedToken>, CreateTokenCommandHandler>()
      .AddTransient<ICommandHandler<DeleteApiKey, ApiKeyDto?>, DeleteApiKeyHandler>()
      .AddTransient<ICommandHandler<DeleteDictionary, DictionaryDto?>, DeleteDictionaryHandler>()
      .AddTransient<ICommandHandler<DeleteLanguage, LanguageDto?>, DeleteLanguageHandler>()
      .AddTransient<ICommandHandler<DeleteRole, RoleDto?>, DeleteRoleHandler>()
      .AddTransient<ICommandHandler<DeleteTemplate, TemplateDto?>, DeleteTemplateHandler>()
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
      .AddTransient<ICommandHandler<UpdateApiKey, ApiKeyDto?>, UpdateApiKeyHandler>()
      .AddTransient<ICommandHandler<UpdateConfiguration, ConfigurationDto>, UpdateConfigurationHandler>()
      .AddTransient<ICommandHandler<UpdateDictionary, DictionaryDto?>, UpdateDictionaryHandler>()
      .AddTransient<ICommandHandler<UpdateLanguage, LanguageDto?>, UpdateLanguageHandler>()
      .AddTransient<ICommandHandler<UpdateRealm, RealmDto?>, UpdateRealmHandler>()
      .AddTransient<ICommandHandler<UpdateRole, RoleDto?>, UpdateRoleHandler>()
      .AddTransient<ICommandHandler<UpdateTemplate, TemplateDto?>, UpdateTemplateHandler>()
      .AddTransient<ICommandHandler<UpdateUser, UserDto?>, UpdateUserHandler>()
      .AddTransient<ICommandHandler<ValidateOneTimePassword, OneTimePasswordDto?>, ValidateOneTimePasswordHandler>()
      .AddTransient<ICommandHandler<ValidateToken, ValidatedToken>, ValidateTokenCommandHandler>();
  }

  public static IServiceCollection AddKrakenarCoreServices(this IServiceCollection services)
  {
    return services
      .AddTransient<IApiKeyService, ApiKeyService>()
      .AddTransient<IConfigurationService, ConfigurationService>()
      .AddTransient<IDictionaryService, DictionaryService>()
      .AddTransient<ILanguageService, LanguageService>()
      .AddTransient<IOneTimePasswordService, OneTimePasswordService>()
      .AddTransient<IRealmService, RealmService>()
      .AddTransient<IRoleService, RoleService>()
      //.AddTransient<ISenderService, SenderService>() // TODO(fpion): implement
      .AddTransient<ISessionService, SessionService>()
      .AddTransient<ITemplateService, TemplateService>()
      .AddTransient<ITokenService, TokenService>()
      .AddTransient<IUserService, UserService>();
  }

  public static IServiceCollection AddKrakenarManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<IDictionaryManager, DictionaryManager>()
      .AddTransient<ILanguageManager, LanguageManager>()
      .AddTransient<IRealmManager, RealmManager>()
      .AddTransient<IRoleManager, RoleManager>()
      .AddTransient<ITemplateManager, TemplateManager>()
      .AddTransient<IUserManager, UserManager>();
  }

  public static IServiceCollection AddKrakenarQueries(this IServiceCollection services)
  {
    return services
      .AddTransient<IQueryHandler<ReadApiKey, ApiKeyDto?>, ReadApiKeyHandler>()
      .AddTransient<IQueryHandler<ReadConfiguration, ConfigurationDto>, ReadConfigurationHandler>()
      .AddTransient<IQueryHandler<ReadDictionary, DictionaryDto?>, ReadDictionaryHandler>()
      .AddTransient<IQueryHandler<ReadLanguage, LanguageDto?>, ReadLanguageHandler>()
      .AddTransient<IQueryHandler<ReadOneTimePassword, OneTimePasswordDto?>, ReadOneTimePasswordHandler>()
      .AddTransient<IQueryHandler<ReadRealm, RealmDto?>, ReadRealmHandler>()
      .AddTransient<IQueryHandler<ReadRole, RoleDto?>, ReadRoleHandler>()
      .AddTransient<IQueryHandler<ReadSession, SessionDto?>, ReadSessionHandler>()
      .AddTransient<IQueryHandler<ReadTemplate, TemplateDto?>, ReadTemplateHandler>()
      .AddTransient<IQueryHandler<ReadUser, UserDto?>, ReadUserHandler>()
      .AddTransient<IQueryHandler<SearchApiKeys, SearchResults<ApiKeyDto>>, SearchApiKeysHandler>()
      .AddTransient<IQueryHandler<SearchDictionaries, SearchResults<DictionaryDto>>, SearchDictionariesHandler>()
      .AddTransient<IQueryHandler<SearchLanguages, SearchResults<LanguageDto>>, SearchLanguagesHandler>()
      .AddTransient<IQueryHandler<SearchRealms, SearchResults<RealmDto>>, SearchRealmsHandler>()
      .AddTransient<IQueryHandler<SearchRoles, SearchResults<RoleDto>>, SearchRolesHandler>()
      .AddTransient<IQueryHandler<SearchSessions, SearchResults<SessionDto>>, SearchSessionsHandler>()
      .AddTransient<IQueryHandler<SearchTemplates, SearchResults<TemplateDto>>, SearchTemplatesHandler>()
      .AddTransient<IQueryHandler<SearchUsers, SearchResults<UserDto>>, SearchUsersHandler>();
  }

  public static IServiceCollection AddKrakenarRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IApiKeyRepository, ApiKeyRepository>()
      .AddTransient<IConfigurationRepository, ConfigurationRepository>()
      .AddTransient<IDictionaryRepository, DictionaryRepository>()
      .AddTransient<ILanguageRepository, LanguageRepository>()
      .AddTransient<IOneTimePasswordRepository, OneTimePasswordRepository>()
      .AddTransient<IRealmRepository, RealmRepository>()
      .AddTransient<IRoleRepository, RoleRepository>()
      .AddTransient<ISenderRepository, SenderRepository>()
      .AddTransient<ISessionRepository, SessionRepository>()
      .AddTransient<ITemplateRepository, TemplateRepository>()
      .AddTransient<IUserRepository, UserRepository>();
  }
}
