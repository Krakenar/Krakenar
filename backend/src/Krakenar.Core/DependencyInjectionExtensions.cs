using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Messages;
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
using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Commands;
using Krakenar.Core.Contents.Queries;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Dictionaries.Commands;
using Krakenar.Core.Dictionaries.Queries;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Commands;
using Krakenar.Core.Fields.Queries;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Commands;
using Krakenar.Core.Localization.Queries;
using Krakenar.Core.Messages;
using Krakenar.Core.Messages.Commands;
using Krakenar.Core.Messages.Queries;
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
using Krakenar.Core.Senders.Queries;
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
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;
using LanguageDto = Krakenar.Contracts.Localization.Language;
using MessageDto = Krakenar.Contracts.Messages.Message;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using RoleDto = Krakenar.Contracts.Roles.Role;
using SenderDto = Krakenar.Contracts.Senders.Sender;
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
      .AddTransient<ICommandHandler<CreateOrReplaceContentType, CreateOrReplaceContentTypeResult>, CreateOrReplaceContentTypeHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceDictionary, CreateOrReplaceDictionaryResult>, CreateOrReplaceDictionaryHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceFieldDefinition, ContentTypeDto?>, CreateOrReplaceFieldDefinitionHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceFieldType, CreateOrReplaceFieldTypeResult>, CreateOrReplaceFieldTypeHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult>, CreateOrReplaceLanguageHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult>, CreateOrReplaceRealmHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult>, CreateOrReplaceRoleHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceSender, CreateOrReplaceSenderResult>, CreateOrReplaceSenderHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceTemplate, CreateOrReplaceTemplateResult>, CreateOrReplaceTemplateHandler>()
      .AddTransient<ICommandHandler<CreateOrReplaceUser, CreateOrReplaceUserResult>, CreateOrReplaceUserHandler>()
      .AddTransient<ICommandHandler<CreateSession, SessionDto>, CreateSessionHandler>()
      .AddTransient<ICommandHandler<CreateToken, CreatedToken>, CreateTokenCommandHandler>()
      .AddTransient<ICommandHandler<DeleteApiKey, ApiKeyDto?>, DeleteApiKeyHandler>()
      .AddTransient<ICommandHandler<DeleteContentType, ContentTypeDto?>, DeleteContentTypeHandler>()
      .AddTransient<ICommandHandler<DeleteDictionary, DictionaryDto?>, DeleteDictionaryHandler>()
      .AddTransient<ICommandHandler<DeleteFieldDefinition, ContentTypeDto?>, DeleteFieldDefinitionHandler>()
      .AddTransient<ICommandHandler<DeleteFieldType, FieldTypeDto?>, DeleteFieldTypeHandler>()
      .AddTransient<ICommandHandler<DeleteLanguage, LanguageDto?>, DeleteLanguageHandler>()
      .AddTransient<ICommandHandler<DeleteRole, RoleDto?>, DeleteRoleHandler>()
      .AddTransient<ICommandHandler<DeleteSender, SenderDto?>, DeleteSenderHandler>()
      .AddTransient<ICommandHandler<DeleteTemplate, TemplateDto?>, DeleteTemplateHandler>()
      .AddTransient<ICommandHandler<DeleteUser, UserDto?>, DeleteUserHandler>()
      .AddTransient<ICommandHandler<InitializeConfiguration>, InitializeConfigurationHandler>()
      .AddTransient<ICommandHandler<RemoveUserIdentifier, UserDto?>, RemoveUserIdentifierHandler>()
      .AddTransient<ICommandHandler<RenewSession, SessionDto>, RenewSessionHandler>()
      .AddTransient<ICommandHandler<ReplaceConfiguration, ConfigurationDto>, ReplaceConfigurationHandler>()
      .AddTransient<ICommandHandler<ResetUserPassword, UserDto?>, ResetUserPasswordHandler>()
      .AddTransient<ICommandHandler<SaveUserIdentifier, UserDto?>, SaveUserIdentifierHandler>()
      .AddTransient<ICommandHandler<SendMessage, SentMessages>, SendMessageHandler>()
      .AddTransient<ICommandHandler<SetDefaultLanguage, LanguageDto?>, SetDefaultLanguageHandler>()
      .AddTransient<ICommandHandler<SetDefaultSender, SenderDto?>, SetDefaultSenderHandler>()
      .AddTransient<ICommandHandler<SignInSession, SessionDto>, SignInSessionHandler>()
      .AddTransient<ICommandHandler<SignOutSession, SessionDto?>, SignOutSessionHandler>()
      .AddTransient<ICommandHandler<SignOutUser, UserDto?>, SignOutUserHandler>()
      .AddTransient<ICommandHandler<UpdateApiKey, ApiKeyDto?>, UpdateApiKeyHandler>()
      .AddTransient<ICommandHandler<UpdateConfiguration, ConfigurationDto>, UpdateConfigurationHandler>()
      .AddTransient<ICommandHandler<UpdateContentType, ContentTypeDto?>, UpdateContentTypeHandler>()
      .AddTransient<ICommandHandler<UpdateDictionary, DictionaryDto?>, UpdateDictionaryHandler>()
      .AddTransient<ICommandHandler<UpdateFieldDefinition, ContentTypeDto?>, UpdateFieldDefinitionHandler>()
      .AddTransient<ICommandHandler<UpdateFieldType, FieldTypeDto?>, UpdateFieldTypeHandler>()
      .AddTransient<ICommandHandler<UpdateLanguage, LanguageDto?>, UpdateLanguageHandler>()
      .AddTransient<ICommandHandler<UpdateRealm, RealmDto?>, UpdateRealmHandler>()
      .AddTransient<ICommandHandler<UpdateRole, RoleDto?>, UpdateRoleHandler>()
      .AddTransient<ICommandHandler<UpdateSender, SenderDto?>, UpdateSenderHandler>()
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
      .AddTransient<IContentTypeService, ContentTypeService>()
      .AddTransient<IDictionaryService, DictionaryService>()
      .AddTransient<IFieldDefinitionService, FieldDefinitionService>()
      .AddTransient<IFieldTypeService, FieldTypeService>()
      .AddTransient<ILanguageService, LanguageService>()
      .AddTransient<IMessageService, MessageService>()
      .AddTransient<IOneTimePasswordService, OneTimePasswordService>()
      .AddTransient<IRealmService, RealmService>()
      .AddTransient<IRoleService, RoleService>()
      .AddTransient<ISenderService, SenderService>()
      .AddTransient<ISessionService, SessionService>()
      .AddTransient<ITemplateService, TemplateService>()
      .AddTransient<ITokenService, TokenService>()
      .AddTransient<IUserService, UserService>();
  }

  public static IServiceCollection AddKrakenarManagers(this IServiceCollection services)
  {
    return services
      .AddTransient<IContentManager, ContentManager>()
      .AddTransient<IDictionaryManager, DictionaryManager>()
      .AddTransient<IFieldManager, FieldManager>()
      .AddTransient<ILanguageManager, LanguageManager>()
      .AddTransient<IRealmManager, RealmManager>()
      .AddTransient<IRoleManager, RoleManager>()
      .AddTransient<ISenderManager, SenderManager>()
      .AddTransient<ITemplateManager, TemplateManager>()
      .AddTransient<IUserManager, UserManager>();
  }

  public static IServiceCollection AddKrakenarQueries(this IServiceCollection services)
  {
    return services
      .AddTransient<IQueryHandler<ReadApiKey, ApiKeyDto?>, ReadApiKeyHandler>()
      .AddTransient<IQueryHandler<ReadConfiguration, ConfigurationDto>, ReadConfigurationHandler>()
      .AddTransient<IQueryHandler<ReadContentType, ContentTypeDto?>, ReadContentTypeHandler>()
      .AddTransient<IQueryHandler<ReadDictionary, DictionaryDto?>, ReadDictionaryHandler>()
      .AddTransient<IQueryHandler<ReadFieldType, FieldTypeDto?>, ReadFieldTypeHandler>()
      .AddTransient<IQueryHandler<ReadLanguage, LanguageDto?>, ReadLanguageHandler>()
      .AddTransient<IQueryHandler<ReadMessage, MessageDto?>, ReadMessageHandler>()
      .AddTransient<IQueryHandler<ReadOneTimePassword, OneTimePasswordDto?>, ReadOneTimePasswordHandler>()
      .AddTransient<IQueryHandler<ReadRealm, RealmDto?>, ReadRealmHandler>()
      .AddTransient<IQueryHandler<ReadRole, RoleDto?>, ReadRoleHandler>()
      .AddTransient<IQueryHandler<ReadSender, SenderDto?>, ReadSenderHandler>()
      .AddTransient<IQueryHandler<ReadSession, SessionDto?>, ReadSessionHandler>()
      .AddTransient<IQueryHandler<ReadTemplate, TemplateDto?>, ReadTemplateHandler>()
      .AddTransient<IQueryHandler<ReadUser, UserDto?>, ReadUserHandler>()
      .AddTransient<IQueryHandler<SearchApiKeys, SearchResults<ApiKeyDto>>, SearchApiKeysHandler>()
      .AddTransient<IQueryHandler<SearchContentTypes, SearchResults<ContentTypeDto>>, SearchContentTypesHandler>()
      .AddTransient<IQueryHandler<SearchDictionaries, SearchResults<DictionaryDto>>, SearchDictionariesHandler>()
      .AddTransient<IQueryHandler<SearchFieldTypes, SearchResults<FieldTypeDto>>, SearchFieldTypesHandler>()
      .AddTransient<IQueryHandler<SearchLanguages, SearchResults<LanguageDto>>, SearchLanguagesHandler>()
      .AddTransient<IQueryHandler<SearchMessages, SearchResults<MessageDto>>, SearchMessagesHandler>()
      .AddTransient<IQueryHandler<SearchRealms, SearchResults<RealmDto>>, SearchRealmsHandler>()
      .AddTransient<IQueryHandler<SearchRoles, SearchResults<RoleDto>>, SearchRolesHandler>()
      .AddTransient<IQueryHandler<SearchSenders, SearchResults<SenderDto>>, SearchSendersHandler>()
      .AddTransient<IQueryHandler<SearchSessions, SearchResults<SessionDto>>, SearchSessionsHandler>()
      .AddTransient<IQueryHandler<SearchTemplates, SearchResults<TemplateDto>>, SearchTemplatesHandler>()
      .AddTransient<IQueryHandler<SearchUsers, SearchResults<UserDto>>, SearchUsersHandler>();
  }

  public static IServiceCollection AddKrakenarRepositories(this IServiceCollection services)
  {
    return services
      .AddTransient<IApiKeyRepository, ApiKeyRepository>()
      .AddTransient<IConfigurationRepository, ConfigurationRepository>()
      .AddTransient<IContentTypeRepository, ContentTypeRepository>()
      .AddTransient<IDictionaryRepository, DictionaryRepository>()
      .AddTransient<IFieldTypeRepository, FieldTypeRepository>()
      .AddTransient<ILanguageRepository, LanguageRepository>()
      .AddTransient<IMessageRepository, MessageRepository>()
      .AddTransient<IOneTimePasswordRepository, OneTimePasswordRepository>()
      .AddTransient<IRealmRepository, RealmRepository>()
      .AddTransient<IRoleRepository, RoleRepository>()
      .AddTransient<ISenderRepository, SenderRepository>()
      .AddTransient<ISessionRepository, SessionRepository>()
      .AddTransient<ITemplateRepository, TemplateRepository>()
      .AddTransient<IUserRepository, UserRepository>();
  }
}
