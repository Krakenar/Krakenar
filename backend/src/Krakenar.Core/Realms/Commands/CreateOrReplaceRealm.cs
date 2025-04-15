using FluentValidation;
using Krakenar.Contracts.Realms;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms.Validators;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms.Commands;

public record CreateOrReplaceRealmResult(RealmDto? Realm = null, bool Created = false);

public record CreateOrReplaceRealm(Guid? Id, CreateOrReplaceRealmPayload Payload, long? Version) : ICommand<CreateOrReplaceRealmResult>;

/// <exception cref="UniqueSlugAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceRealmHandler : ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual ILanguageRepository LanguageRepository { get; }
  protected virtual IRealmQuerier RealmQuerier { get; }
  protected virtual IRealmRepository RealmRepository { get; }
  protected virtual IRealmService RealmService { get; }
  protected virtual ISecretService SecretService { get; }

  public CreateOrReplaceRealmHandler(
    IApplicationContext applicationContext,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository,
    IRealmService realmService,
    ISecretService secretService)
  {
    ApplicationContext = applicationContext;
    LanguageQuerier = languageQuerier;
    LanguageRepository = languageRepository;
    RealmQuerier = realmQuerier;
    RealmRepository = realmRepository;
    RealmService = realmService;
    SecretService = secretService;
  }

  public virtual async Task<CreateOrReplaceRealmResult> HandleAsync(CreateOrReplaceRealm command, CancellationToken cancellationToken)
  {
    CreateOrReplaceRealmPayload payload = command.Payload;
    new CreateOrReplaceRealmValidator().ValidateAndThrow(payload);

    RealmId realmId = RealmId.NewId();
    Realm? realm = null;
    if (command.Id.HasValue)
    {
      realmId = new(command.Id.Value);
      realm = await RealmRepository.LoadAsync(realmId, cancellationToken);
    }

    Slug uniqueSlug = new(payload.UniqueSlug);
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    Language? language = null;
    if (realm is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceRealmResult();
      }

      Secret secret = SecretService.Generate(realmId);
      realm = new(uniqueSlug, secret, actorId, realmId);
      created = true;

      Locale locale = await LanguageQuerier.FindPlatformDefaultLocaleAsync(cancellationToken);
      language = new Language(locale, isDefault: true, actorId, LanguageId.NewId(realm.Id));
    }

    Realm reference = (command.Version.HasValue
      ? await RealmRepository.LoadAsync(realmId, command.Version.Value, cancellationToken)
      : null) ?? realm;

    if (reference.UniqueSlug != uniqueSlug)
    {
      realm.SetUniqueSlug(uniqueSlug, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      realm.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      realm.Description = description;
    }

    Url? url = Url.TryCreate(payload.Url);
    if (reference.Url != url)
    {
      realm.Url = url;
    }

    UniqueNameSettings uniqueNameSettings = new(payload.UniqueNameSettings);
    if (reference.UniqueNameSettings != uniqueNameSettings)
    {
      realm.UniqueNameSettings = uniqueNameSettings;
    }
    PasswordSettings passwordSettings = new(payload.PasswordSettings);
    if (reference.PasswordSettings != passwordSettings)
    {
      realm.PasswordSettings = passwordSettings;
    }
    if (reference.RequireUniqueEmail != payload.RequireUniqueEmail)
    {
      realm.RequireUniqueEmail = payload.RequireUniqueEmail;
    }
    if (reference.RequireConfirmedAccount != payload.RequireConfirmedAccount)
    {
      realm.RequireConfirmedAccount = payload.RequireConfirmedAccount;
    }

    realm.SetCustomAttributes(payload.CustomAttributes, reference);

    realm.Update(actorId);
    await RealmService.SaveAsync(realm, cancellationToken);

    if (language is not null)
    {
      await LanguageRepository.SaveAsync(language, cancellationToken);
    }

    RealmDto dto = await RealmQuerier.ReadAsync(realm, cancellationToken);
    return new CreateOrReplaceRealmResult(dto, created);
  }
}
