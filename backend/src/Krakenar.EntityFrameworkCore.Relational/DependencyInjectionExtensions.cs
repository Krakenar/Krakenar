using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.Core.ApiKeys;
using Krakenar.Core.ApiKeys.Events;
using Krakenar.Core.Configurations;
using Krakenar.Core.Configurations.Events;
using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Dictionaries.Events;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Events;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Events;
using Krakenar.Core.Messages;
using Krakenar.Core.Messages.Events;
using Krakenar.Core.Passwords;
using Krakenar.Core.Passwords.Events;
using Krakenar.Core.Realms;
using Krakenar.Core.Realms.Events;
using Krakenar.Core.Roles;
using Krakenar.Core.Roles.Events;
using Krakenar.Core.Senders;
using Krakenar.Core.Senders.Events;
using Krakenar.Core.Sessions;
using Krakenar.Core.Sessions.Events;
using Krakenar.Core.Templates;
using Krakenar.Core.Templates.Events;
using Krakenar.Core.Tokens;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Events;
using Krakenar.EntityFrameworkCore.Relational.Actors;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Krakenar.EntityFrameworkCore.Relational.Queriers;
using Krakenar.EntityFrameworkCore.Relational.Tokens;
using Krakenar.Infrastructure.Commands;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.Extensions.DependencyInjection;

namespace Krakenar.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddKrakenarEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddKrakenarEventHandlers()
      .AddKrakenarQueriers()
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddScoped<IActorService, ActorService>()
      .AddScoped<ITokenBlacklist, TokenBlacklist>()
      .AddScoped<ICommandHandler<MigrateDatabase>, MigrateDatabaseCommandHandler>();
  }

  public static IServiceCollection AddKrakenarEventHandlers(this IServiceCollection services)
  {
    return services
      .AddScoped<IEventHandler<ApiKeyAuthenticated>, ApiKeyEvents>()
      .AddScoped<IEventHandler<ApiKeyCreated>, ApiKeyEvents>()
      .AddScoped<IEventHandler<ApiKeyDeleted>, ApiKeyEvents>()
      .AddScoped<IEventHandler<ApiKeyDeleted>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<ApiKeyRoleAdded>, ApiKeyEvents>()
      .AddScoped<IEventHandler<ApiKeyRoleRemoved>, ApiKeyEvents>()
      .AddScoped<IEventHandler<ApiKeyUpdated>, ApiKeyEvents>()
      .AddScoped<IEventHandler<ApiKeyUpdated>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<ConfigurationInitialized>, ConfigurationEvents>()
      .AddScoped<IEventHandler<ConfigurationUpdated>, ConfigurationEvents>()
      .AddScoped<IEventHandler<ContentCreated>, ContentEvents>()
      .AddScoped<IEventHandler<ContentDeleted>, ContentEvents>()
      .AddScoped<IEventHandler<ContentLocaleChanged>, ContentEvents>()
      .AddScoped<IEventHandler<ContentLocalePublished>, ContentEvents>()
      .AddScoped<IEventHandler<ContentLocaleRemoved>, ContentEvents>()
      .AddScoped<IEventHandler<ContentLocaleUnpublished>, ContentEvents>()
      .AddScoped<IEventHandler<ContentTypeCreated>, ContentTypeEvents>()
      .AddScoped<IEventHandler<ContentTypeDeleted>, ContentTypeEvents>()
      .AddScoped<IEventHandler<ContentTypeFieldChanged>, ContentTypeEvents>()
      .AddScoped<IEventHandler<ContentTypeFieldRemoved>, ContentTypeEvents>()
      .AddScoped<IEventHandler<ContentTypeUniqueNameChanged>, ContentTypeEvents>()
      .AddScoped<IEventHandler<ContentTypeUpdated>, ContentTypeEvents>()
      .AddScoped<IEventHandler<DictionaryCreated>, DictionaryEvents>()
      .AddScoped<IEventHandler<DictionaryDeleted>, DictionaryEvents>()
      .AddScoped<IEventHandler<DictionaryLanguageChanged>, DictionaryEvents>()
      .AddScoped<IEventHandler<DictionaryUpdated>, DictionaryEvents>()
      .AddScoped<IEventHandler<EmailSenderCreated>, SenderEvents>()
      .AddScoped<IEventHandler<FieldTypeBooleanSettingsChanged>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeCreated>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeDateTimeSettingsChanged>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeDeleted>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeNumberSettingsChanged>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeRelatedContentSettingsChanged>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeRichTextSettingsChanged>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeSelectSettingsChanged>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeStringSettingsChanged>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeTagsSettingsChanged>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeUniqueNameChanged>, FieldTypeEvents>()
      .AddScoped<IEventHandler<FieldTypeUpdated>, FieldTypeEvents>()
      .AddScoped<IEventHandler<LanguageCreated>, LanguageEvents>()
      .AddScoped<IEventHandler<LanguageDeleted>, LanguageEvents>()
      .AddScoped<IEventHandler<LanguageLocaleChanged>, LanguageEvents>()
      .AddScoped<IEventHandler<LanguageSetDefault>, LanguageEvents>()
      .AddScoped<IEventHandler<MessageCreated>, MessageEvents>()
      .AddScoped<IEventHandler<MessageDeleted>, MessageEvents>()
      .AddScoped<IEventHandler<MessageFailed>, MessageEvents>()
      .AddScoped<IEventHandler<MessageSucceeded>, MessageEvents>()
      .AddScoped<IEventHandler<OneTimePasswordCreated>, OneTimePasswordEvents>()
      .AddScoped<IEventHandler<OneTimePasswordDeleted>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<OneTimePasswordDeleted>, OneTimePasswordEvents>()
      .AddScoped<IEventHandler<OneTimePasswordUpdated>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<OneTimePasswordUpdated>, OneTimePasswordEvents>()
      .AddScoped<IEventHandler<OneTimePasswordValidationFailed>, OneTimePasswordEvents>()
      .AddScoped<IEventHandler<OneTimePasswordValidationSucceeded>, OneTimePasswordEvents>()
      .AddScoped<IEventHandler<PhoneSenderCreated>, SenderEvents>()
      .AddScoped<IEventHandler<RealmCreated>, RealmEvents>()
      .AddScoped<IEventHandler<RealmDeleted>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<RealmDeleted>, RealmEvents>()
      .AddScoped<IEventHandler<RealmUniqueSlugChanged>, RealmEvents>()
      .AddScoped<IEventHandler<RealmUpdated>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<RealmUpdated>, RealmEvents>()
      .AddScoped<IEventHandler<RoleCreated>, RoleEvents>()
      .AddScoped<IEventHandler<RoleDeleted>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<RoleDeleted>, RoleEvents>()
      .AddScoped<IEventHandler<RoleUniqueNameChanged>, RoleEvents>()
      .AddScoped<IEventHandler<RoleUpdated>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<RoleUpdated>, RoleEvents>()
      .AddScoped<IEventHandler<SenderDeleted>, SenderEvents>()
      .AddScoped<IEventHandler<SenderSetDefault>, SenderEvents>()
      .AddScoped<IEventHandler<SenderUpdated>, SenderEvents>()
      .AddScoped<IEventHandler<SendGridSettingsChanged>, SenderEvents>()
      .AddScoped<IEventHandler<SessionCreated>, SessionEvents>()
      .AddScoped<IEventHandler<SessionDeleted>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<SessionDeleted>, SessionEvents>()
      .AddScoped<IEventHandler<SessionRenewed>, SessionEvents>()
      .AddScoped<IEventHandler<SessionSignedOut>, SessionEvents>()
      .AddScoped<IEventHandler<SessionUpdated>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<SessionUpdated>, SessionEvents>()
      .AddScoped<IEventHandler<TemplateCreated>, TemplateEvents>()
      .AddScoped<IEventHandler<TemplateDeleted>, TemplateEvents>()
      .AddScoped<IEventHandler<TemplateUniqueNameChanged>, TemplateEvents>()
      .AddScoped<IEventHandler<TemplateUpdated>, TemplateEvents>()
      .AddScoped<IEventHandler<TwilioSettingsChanged>, SenderEvents>()
      .AddScoped<IEventHandler<UserAddressChanged>, UserEvents>()
      .AddScoped<IEventHandler<UserAuthenticated>, UserEvents>()
      .AddScoped<IEventHandler<UserCreated>, UserEvents>()
      .AddScoped<IEventHandler<UserDeleted>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<UserDeleted>, UserEvents>()
      .AddScoped<IEventHandler<UserDisabled>, UserEvents>()
      .AddScoped<IEventHandler<UserEmailChanged>, UserEvents>()
      .AddScoped<IEventHandler<UserEnabled>, UserEvents>()
      .AddScoped<IEventHandler<UserIdentifierChanged>, UserEvents>()
      .AddScoped<IEventHandler<UserIdentifierRemoved>, UserEvents>()
      .AddScoped<IEventHandler<UserPasswordChanged>, UserEvents>()
      .AddScoped<IEventHandler<UserPasswordRemoved>, UserEvents>()
      .AddScoped<IEventHandler<UserPasswordReset>, UserEvents>()
      .AddScoped<IEventHandler<UserPasswordUpdated>, UserEvents>()
      .AddScoped<IEventHandler<UserPhoneChanged>, UserEvents>()
      .AddScoped<IEventHandler<UserRoleAdded>, UserEvents>()
      .AddScoped<IEventHandler<UserRoleRemoved>, UserEvents>()
      .AddScoped<IEventHandler<UserSignedIn>, UserEvents>()
      .AddScoped<IEventHandler<UserUniqueNameChanged>, UserEvents>()
      .AddScoped<IEventHandler<UserUpdated>, CustomAttributeEvents>()
      .AddScoped<IEventHandler<UserUpdated>, UserEvents>();
  }

  public static IServiceCollection AddKrakenarQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IApiKeyQuerier, ApiKeyQuerier>()
      .AddScoped<IConfigurationQuerier, ConfigurationQuerier>()
      .AddScoped<IContentQuerier, ContentQuerier>()
      .AddScoped<IContentLocaleQuerier, ContentLocaleQuerier>()
      .AddScoped<IContentTypeQuerier, ContentTypeQuerier>()
      .AddScoped<IDictionaryQuerier, DictionaryQuerier>()
      .AddScoped<IFieldTypeQuerier, FieldTypeQuerier>()
      .AddScoped<ILanguageQuerier, LanguageQuerier>()
      .AddScoped<IMessageQuerier, MessageQuerier>()
      .AddScoped<IOneTimePasswordQuerier, OneTimePasswordQuerier>()
      .AddScoped<IRealmQuerier, RealmQuerier>()
      .AddScoped<IRoleQuerier, RoleQuerier>()
      .AddScoped<ISenderQuerier, SenderQuerier>()
      .AddScoped<ISessionQuerier, SessionQuerier>()
      .AddScoped<ITemplateQuerier, TemplateQuerier>()
      .AddScoped<IUserQuerier, UserQuerier>();
  }
}
