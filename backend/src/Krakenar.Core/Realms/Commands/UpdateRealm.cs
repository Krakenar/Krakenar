﻿using FluentValidation;
using Krakenar.Contracts;
using Krakenar.Contracts.Realms;
using Krakenar.Core.Realms.Validators;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms.Commands;

public record UpdateRealm(Guid Id, UpdateRealmPayload Payload) : ICommand<RealmDto?>;

/// <exception cref="UniqueSlugAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateRealmHandler : ICommandHandler<UpdateRealm, RealmDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IRealmQuerier RealmQuerier { get; }
  protected virtual IRealmRepository RealmRepository { get; }
  protected virtual IRealmService RealmService { get; }

  public UpdateRealmHandler(
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
    await RealmService.SaveAsync(realm, cancellationToken);

    return await RealmQuerier.ReadAsync(realm, cancellationToken);
  }
}
