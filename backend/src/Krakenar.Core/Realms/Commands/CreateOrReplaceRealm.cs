using FluentValidation;
using Krakenar.Contracts.Realms;
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
  protected virtual IRealmQuerier RealmQuerier { get; }
  protected virtual IRealmRepository RealmRepository { get; }
  protected virtual IRealmService RealmService { get; }

  public CreateOrReplaceRealmHandler(
    IApplicationContext applicationContext,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository,
    IRealmService realmService)
  {
    ApplicationContext = applicationContext;
    RealmQuerier = realmQuerier;
    RealmRepository = realmRepository;
    RealmService = realmService;
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
    Secret secret = null!; // TODO(fpion): Secret
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    if (realm is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceRealmResult();
      }

      realm = new(uniqueSlug, secret, actorId, realmId);
      created = true;
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

    if (reference.Secret != secret)
    {
      realm.Secret = secret;
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
      reference.RequireUniqueEmail = payload.RequireUniqueEmail;
    }
    if (reference.RequireConfirmedAccount != payload.RequireConfirmedAccount)
    {
      reference.RequireConfirmedAccount = payload.RequireConfirmedAccount;
    }

    realm.SetCustomAttributes(payload.CustomAttributes, reference);

    realm.Update(actorId);
    await RealmService.SaveAsync(realm, cancellationToken);

    // TODO(fpion): Default Language

    RealmDto dto = await RealmQuerier.ReadAsync(realm, cancellationToken);
    return new CreateOrReplaceRealmResult(dto, created);
  }
}
