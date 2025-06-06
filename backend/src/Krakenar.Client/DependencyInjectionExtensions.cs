﻿using Krakenar.Client.ApiKeys;
using Krakenar.Client.Configurations;
using Krakenar.Client.Contents;
using Krakenar.Client.Dictionaries;
using Krakenar.Client.Fields;
using Krakenar.Client.Localization;
using Krakenar.Client.Messages;
using Krakenar.Client.Passwords;
using Krakenar.Client.Realms;
using Krakenar.Client.Roles;
using Krakenar.Client.Senders;
using Krakenar.Client.Sessions;
using Krakenar.Client.Templates;
using Krakenar.Client.Tokens;
using Krakenar.Client.Users;
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
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Templates;
using Krakenar.Contracts.Tokens;
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
      .AddKrakenarClients()
      .AddKrakenarClientServices();
  }

  public static IServiceCollection AddKrakenarClients(this IServiceCollection services)
  {
    return services
      .AddSingleton<IContentClient, ContentClient>()
      .AddSingleton<IOneTimePasswordClient, OneTimePasswordClient>()
      .AddSingleton<ISessionClient, SessionClient>()
      .AddSingleton<IUserClient, UserClient>();
  }

  public static IServiceCollection AddKrakenarClientServices(this IServiceCollection services)
  {
    return services
      .AddSingleton<IApiKeyService, ApiKeyClient>()
      .AddSingleton<IConfigurationService, ConfigurationClient>()
      .AddSingleton<IContentService, ContentClient>()
      .AddSingleton<IContentTypeService, ContentTypeClient>()
      .AddSingleton<IDictionaryService, DictionaryClient>()
      .AddSingleton<IFieldDefinitionService, FieldDefinitionClient>()
      .AddSingleton<IFieldTypeService, FieldTypeClient>()
      .AddSingleton<ILanguageService, LanguageClient>()
      .AddSingleton<IMessageService, MessageClient>()
      .AddSingleton<IOneTimePasswordService, OneTimePasswordClient>()
      .AddSingleton<IPublishedContentService, PublishedContentClient>()
      .AddSingleton<IRealmService, RealmClient>()
      .AddSingleton<IRoleService, RoleClient>()
      .AddSingleton<ISenderService, SenderClient>()
      .AddSingleton<ISessionService, SessionClient>()
      .AddSingleton<ITemplateService, TemplateClient>()
      .AddSingleton<ITokenService, TokenClient>()
      .AddSingleton<IUserService, UserClient>();
  }
}
