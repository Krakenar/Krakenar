﻿using Krakenar.Contracts;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Realms;
using Krakenar.Core.Realms.Commands;
using Krakenar.Core.Realms.Queries;
using Krakenar.Core.Tokens;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using PasswordSettingsDto = Krakenar.Contracts.Settings.PasswordSettings;
using Realm = Krakenar.Core.Realms.Realm;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using UniqueNameSettingsDto = Krakenar.Contracts.Settings.UniqueNameSettings;

namespace Krakenar.Realms;

[Trait(Traits.Category, Categories.Integration)]
public class RealmIntegrationTests : IntegrationTests
{
  private readonly IRealmRepository _realmRepository;
  private readonly ISecretService _secretService;

  private readonly ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult> _createOrReplaceRealm;
  private readonly IQueryHandler<ReadRealm, RealmDto?> _readRealm;
  private readonly IQueryHandler<SearchRealms, SearchResults<RealmDto>> _searchRealms;
  private readonly ICommandHandler<UpdateRealm, RealmDto?> _updateRealm;

  public RealmIntegrationTests() : base()
  {
    _realmRepository = ServiceProvider.GetRequiredService<IRealmRepository>();
    _secretService = ServiceProvider.GetRequiredService<ISecretService>();

    _createOrReplaceRealm = ServiceProvider.GetRequiredService<ICommandHandler<CreateOrReplaceRealm, CreateOrReplaceRealmResult>>();
    _readRealm = ServiceProvider.GetRequiredService<IQueryHandler<ReadRealm, RealmDto?>>();
    _searchRealms = ServiceProvider.GetRequiredService<IQueryHandler<SearchRealms, SearchResults<RealmDto>>>();
    _updateRealm = ServiceProvider.GetRequiredService<ICommandHandler<UpdateRealm, RealmDto?>>();
  }

  [Fact(DisplayName = "It should create a new realm.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created()
  {
    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = "new-world",
      DisplayName = " New World ",
      Description = "  This is the new world.  ",
      Url = $"https://www.{Faker.Internet.DomainName()}",
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "Value"));

    CreateOrReplaceRealm command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceRealmResult result = await _createOrReplaceRealm.HandleAsync(command);
    Assert.True(result.Created);

    RealmDto? realm = result.Realm;
    Assert.NotNull(realm);
    Assert.Equal(command.Id, realm.Id);
    Assert.Equal(2, realm.Version);
    Assert.Equal(Actor, realm.CreatedBy);
    Assert.Equal(DateTime.UtcNow, realm.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, realm.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, realm.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName);
    Assert.Equal(payload.Description.Trim(), realm.Description);
    Assert.NotNull(realm.Secret);
    Assert.Equal(payload.Url, realm.Url);
    Assert.Equal(payload.UniqueNameSettings, realm.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.CustomAttributes, realm.CustomAttributes);
  }

  [Fact(DisplayName = "It should read the realm by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    ReadRealm query = new(Realm.Id.ToGuid(), UniqueSlug: null);
    RealmDto? realm = await _readRealm.HandleAsync(query);
    Assert.NotNull(realm);
    Assert.Equal(query.Id, realm.Id);
  }

  [Fact(DisplayName = "It should read the realm by unique slug.")]
  public async Task Given_UniqueSlug_When_Read_Then_Found()
  {
    ReadRealm query = new(Id: null, Realm.UniqueSlug.Value);
    RealmDto? realm = await _readRealm.HandleAsync(query);
    Assert.NotNull(realm);
    Assert.Equal(Realm.Id.ToGuid(), realm.Id);
  }

  [Fact(DisplayName = "It should replace an existing realm.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = "the-new-world",
      DisplayName = " The New World ",
      Description = "  This is the new world.  ",
      Url = $"https://www.{Faker.Internet.DomainName()}",
      UniqueNameSettings = new UniqueNameSettingsDto(allowedCharacters: null),
      PasswordSettings = new PasswordSettingsDto(6, 1, false, true, true, true, "PBKDF2"),
      RequireUniqueEmail = false,
      RequireConfirmedAccount = false
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "Value"));

    CreateOrReplaceRealm command = new(Realm.Id.ToGuid(), payload, Version: null);
    CreateOrReplaceRealmResult result = await _createOrReplaceRealm.HandleAsync(command);
    Assert.False(result.Created);

    RealmDto? realm = result.Realm;
    Assert.NotNull(realm);
    Assert.Equal(Realm.Id.ToGuid(), realm.Id);
    Assert.Equal(Realm.Version + 2, realm.Version);
    Assert.Equal(Actor, realm.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, realm.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName);
    Assert.Equal(payload.Description.Trim(), realm.Description);
    Assert.Equal(Realm.Secret.Value, realm.Secret);
    Assert.Equal(payload.Url, realm.Url);
    Assert.Equal(payload.UniqueNameSettings, realm.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.CustomAttributes, realm.CustomAttributes);
  }

  [Fact(DisplayName = "It should replace an existing realm from reference.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    long version = Realm.Version;

    Description description = new("  This is the new world.  ");
    Realm.Description = description;
    Realm.Update(ActorId);
    await _realmRepository.SaveAsync(Realm);

    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = "the-new-world",
      DisplayName = " The New World ",
      Description = $"  {Realm.Description}  ",
      Url = $"https://www.{Faker.Internet.DomainName()}",
      UniqueNameSettings = new UniqueNameSettingsDto(allowedCharacters: null),
      PasswordSettings = new PasswordSettingsDto(6, 1, false, true, true, true, "PBKDF2"),
      RequireUniqueEmail = false,
      RequireConfirmedAccount = false
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "Value"));

    CreateOrReplaceRealm command = new(Realm.Id.ToGuid(), payload, version);
    CreateOrReplaceRealmResult result = await _createOrReplaceRealm.HandleAsync(command);
    Assert.False(result.Created);

    RealmDto? realm = result.Realm;
    Assert.NotNull(realm);
    Assert.Equal(Realm.Id.ToGuid(), realm.Id);
    Assert.Equal(Realm.Version + 2, realm.Version);
    Assert.Equal(Actor, realm.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, realm.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName);
    Assert.Equal(description.Value, realm.Description);
    Assert.Equal(Realm.Secret.Value, realm.Secret);
    Assert.Equal(payload.Url, realm.Url);
    Assert.Equal(payload.UniqueNameSettings, realm.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, realm.PasswordSettings);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.CustomAttributes, realm.CustomAttributes);
  }

  [Fact(DisplayName = "It should return null when the realm cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    ReadRealm query = new(Guid.Empty, "not-found");
    Assert.Null(await _readRealm.HandleAsync(query));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Realms_When_Search_Then_CorrectResults()
  {
    Realm oldWorld = new(new Slug("old-world"), Realm.Secret);
    Realm underworld = new(new Slug("underworld"), Realm.Secret);
    Realm other = new(new Slug("other-realm"), Realm.Secret);
    await _realmRepository.SaveAsync([oldWorld, underworld, other]);

    SearchRealmsPayload payload = new()
    {
      Ids = [Realm.Id.ToGuid(), oldWorld.Id.ToGuid(), other.Id.ToGuid(), Guid.Empty],
      Search = new TextSearch([new SearchTerm("%world"), new SearchTerm("kraken")], SearchOperator.Or),
      Sort = [new RealmSortOption(RealmSort.UniqueSlug, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    SearchRealms command = new(payload);
    SearchResults<RealmDto> results = await _searchRealms.HandleAsync(command);
    Assert.Equal(2, results.Total);

    RealmDto realm = Assert.Single(results.Items);
    Assert.Equal(Realm.Id.ToGuid(), realm.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple realms were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    RealmId realmId = RealmId.NewId();
    Secret secret = _secretService.Generate(realmId);
    Realm realm = new(new Slug("old-world"), secret, ActorId, realmId);
    await _realmRepository.SaveAsync(realm);

    ReadRealm query = new(Realm.Id.ToGuid(), realm.UniqueSlug.Value);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<RealmDto>>(async () => await _readRealm.HandleAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UniqueSlugAlreadyUsedException when there is a unique slug conflict.")]
  public async Task Given_SlugConflict_When_Read_Then_UniqueSlugAlreadyUsedException()
  {
    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = Realm.UniqueSlug.Value
    };
    CreateOrReplaceRealm command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await _createOrReplaceRealm.HandleAsync(command));

    Assert.Equal(command.Id, exception.RealmId);
    Assert.Equal(Realm.Id.ToGuid(), exception.ConflictId);
    Assert.Equal(payload.UniqueSlug, exception.UniqueSlug);
    Assert.Equal("UniqueSlug", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing realm.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateRealmPayload payload = new()
    {
      DisplayName = new Contracts.Change<string>(" The New World "),
      Url = new Contracts.Change<string>($"https://www.{Faker.Internet.DomainName()}"),
      RequireUniqueEmail = false
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "  Value  "));
    payload.CustomAttributes.Add(new CustomAttribute("Other", string.Empty));
    UpdateRealm command = new(Realm.Id.ToGuid(), payload);
    RealmDto? realm = await _updateRealm.HandleAsync(command);
    Assert.NotNull(realm);

    Assert.Equal(command.Id, realm.Id);
    Assert.Equal(Realm.Version + 1, realm.Version);
    Assert.Equal(Actor, realm.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, realm.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(Realm.UniqueSlug.Value, realm.UniqueSlug);
    Assert.Equal(payload.DisplayName.Value?.Trim(), realm.DisplayName);
    Assert.Equal(Realm.Description?.Value, realm.Description);
    Assert.Equal(Realm.Secret.Value, realm.Secret);
    Assert.Equal(payload.Url.Value, realm.Url);
    Assert.Equal(new UniqueNameSettingsDto(Realm.UniqueNameSettings), realm.UniqueNameSettings);
    Assert.Equal(new PasswordSettingsDto(Realm.PasswordSettings), realm.PasswordSettings);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(Realm.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Single(realm.CustomAttributes);
    Assert.Contains(realm.CustomAttributes, c => c.Key == "Key" && c.Value == "Value");
  }
}
