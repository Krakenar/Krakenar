using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Realms;
using Krakenar.Core.Realms.Validators;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms.Commands;

public record UpdateRealm(Guid Id, UpdateRealmPayload Payload) : ICommand<RealmDto?>;

/// <exception cref="UniqueSlugAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateRealmHandler : ICommandHandler<UpdateRealm, RealmDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IRealmManager RealmManager { get; }
  protected virtual IRealmQuerier RealmQuerier { get; }
  protected virtual IRealmRepository RealmRepository { get; }
  protected virtual ISecretManager SecretManager { get; }

  public UpdateRealmHandler(
    IApplicationContext applicationContext,
    IRealmManager realmManager,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository,
    ISecretManager secretManager)
  {
    ApplicationContext = applicationContext;
    RealmManager = realmManager;
    RealmQuerier = realmQuerier;
    RealmRepository = realmRepository;
    SecretManager = secretManager;
  }

  public virtual async Task<RealmDto?> HandleAsync(UpdateRealm command, CancellationToken cancellationToken)
  {
    UpdateRealmPayload payload = command.Payload;
    new UpdateRealmValidator().ValidateAndThrow(payload);

    RealmId realmId = new(command.Id);
    Realm? realm = await RealmRepository.LoadAsync(realmId, cancellationToken);
    if (realm is null)
    {
      return null;
    }

    ActorId? actorId = ApplicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueSlug))
    {
      Slug uniqueSlug = new(payload.UniqueSlug);
      realm.SetUniqueSlug(uniqueSlug, actorId);
    }
    if (payload.DisplayName is not null)
    {
      realm.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      realm.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Secret is not null)
    {
      Secret secret = string.IsNullOrWhiteSpace(payload.Secret.Value)
        ? SecretManager.Generate(realm.Id)
        : SecretManager.Encrypt(payload.Secret.Value.Trim(), realm.Id);
      realm.SetSecret(secret, actorId);
    }

    if (payload.Url is not null)
    {
      realm.Url = Url.TryCreate(payload.Url.Value);
    }

    if (payload.UniqueNameSettings is not null)
    {
      realm.UniqueNameSettings = new UniqueNameSettings(payload.UniqueNameSettings);
    }
    if (payload.PasswordSettings is not null)
    {
      realm.PasswordSettings = new PasswordSettings(payload.PasswordSettings);
    }
    if (payload.RequireUniqueEmail.HasValue)
    {
      realm.RequireUniqueEmail = payload.RequireUniqueEmail.Value;
    }
    if (payload.RequireConfirmedAccount.HasValue)
    {
      realm.RequireConfirmedAccount = payload.RequireConfirmedAccount.Value;
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Identifier key = new(customAttribute.Key);
      realm.SetCustomAttribute(key, customAttribute.Value);
    }

    realm.Update(actorId);
    await RealmManager.SaveAsync(realm, cancellationToken);

    return await RealmQuerier.ReadAsync(realm, cancellationToken);
  }
}
