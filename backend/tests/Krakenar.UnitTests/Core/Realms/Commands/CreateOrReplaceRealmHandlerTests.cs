using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Realms;
using Krakenar.Core.Localization;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using PasswordSettingsDto = Krakenar.Contracts.Settings.PasswordSettings;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using UniqueNameSettingsDto = Krakenar.Contracts.Settings.UniqueNameSettings;

namespace Krakenar.Core.Realms.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceRealmHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageService> _languageService = new();
  private readonly Mock<IRealmQuerier> _realmQuerier = new();
  private readonly Mock<IRealmRepository> _realmRepository = new();
  private readonly Mock<IRealmService> _realmService = new();
  private readonly Mock<ISecretService> _secretService = new();

  private readonly CreateOrReplaceRealmHandler _handler;

  public CreateOrReplaceRealmHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageQuerier.Object, _languageService.Object, _realmQuerier.Object, _realmRepository.Object, _realmService.Object, _secretService.Object);
  }

  [Theory(DisplayName = "It should create a new realm.")]
  [InlineData(null)]
  [InlineData("be6ec9d2-37f5-4480-95d3-fdfc424ea7ea")]
  public async Task Given_NotExists_When_HandleAsync_Then_Created(string? idValue)
  {
    Guid? id = idValue is null ? null : Guid.Parse(idValue);

    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = "my-new-realm",
      DisplayName = " My New Realm ",
      Description = "  This is my new realm.  ",
      Url = $"https://www.{_faker.Internet.DomainName()}",
      UniqueNameSettings = new UniqueNameSettingsDto("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"),
      PasswordSettings = new PasswordSettingsDto(6, 1, false, true, true, true, "PBKDF2"),
      RequireUniqueEmail = false,
      RequireConfirmedAccount = false
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "Value"));

    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    _secretService.Setup(x => x.Generate(It.Is<RealmId?>(r => r.HasValue))).Returns(secret);

    Locale locale = new("en");
    _languageQuerier.Setup(x => x.FindPlatformDefaultLocaleAsync(_cancellationToken)).ReturnsAsync(locale);

    RealmDto dto = new();
    _realmQuerier.Setup(x => x.ReadAsync(It.IsAny<Realm>(), _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceRealm command = new(id, payload, Version: null);
    CreateOrReplaceRealmResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.Realm);
    Assert.Same(dto, result.Realm);

    if (id.HasValue)
    {
      _realmRepository.Verify(x => x.LoadAsync(It.Is<RealmId>(i => i.ToGuid() == id.Value), _cancellationToken), Times.Once);
    }

    _realmService.Verify(x => x.SaveAsync(
      It.Is<Realm>(r => (!id.HasValue || id.Value == r.Id.ToGuid()) && r.CreatedBy == actorId && r.UpdatedBy == actorId && r.UniqueSlug.Value == payload.UniqueSlug
        && r.DisplayName != null && r.DisplayName.Value == payload.DisplayName.Trim() && r.Description != null && r.Description.Value == payload.Description.Trim()
        && r.Secret.Equals(secret) && r.Url != null && r.Url.Value == payload.Url
        && r.UniqueNameSettings.Equals(new Settings.UniqueNameSettings(payload.UniqueNameSettings)) && r.PasswordSettings.Equals(new Settings.PasswordSettings(payload.PasswordSettings))
        && r.RequireUniqueEmail == payload.RequireUniqueEmail && r.RequireConfirmedAccount == payload.RequireConfirmedAccount
        && r.CustomAttributes.Count == 1 && r.CustomAttributes[new Identifier("Key")] == "Value"),
      _cancellationToken), Times.Once);

    _languageService.Verify(x => x.SaveAsync(It.Is<Language>(l => l.Locale.Equals(locale) && l.IsDefault), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should replace an existing realm.")]
  public async Task Given_Found_When_HandleAsync_Then_Replaced()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Secret secret = new(RandomStringGenerator.GetString(999));
    Realm realm = new(new Slug("old-realm"), secret, actorId);
    _realmRepository.Setup(x => x.LoadAsync(realm.Id, _cancellationToken)).ReturnsAsync(realm);

    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = "new-realm",
      DisplayName = " New Realm ",
      Description = "  This is my new realm!  ",
      Url = $"https://www.{_faker.Internet.DomainName()}",
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "Value"));

    RealmDto dto = new();
    _realmQuerier.Setup(x => x.ReadAsync(realm, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceRealm command = new(realm.Id.ToGuid(), payload, Version: null);
    CreateOrReplaceRealmResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Realm);
    Assert.Same(dto, result.Realm);

    _realmRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<long?>(), It.IsAny<CancellationToken>()), Times.Never);

    _realmService.Verify(x => x.SaveAsync(
      It.Is<Realm>(r => r.Equals(realm) && r.CreatedBy == actorId && r.UpdatedBy == actorId && r.UniqueSlug.Value == payload.UniqueSlug
        && r.DisplayName != null && r.DisplayName.Value == payload.DisplayName.Trim() && r.Description != null && r.Description.Value == payload.Description.Trim()
        && r.Url != null && r.Url.Value == payload.Url
        && r.UniqueNameSettings.Equals(new Settings.UniqueNameSettings(payload.UniqueNameSettings)) && r.PasswordSettings.Equals(new Settings.PasswordSettings(payload.PasswordSettings))
        && r.RequireUniqueEmail == payload.RequireUniqueEmail && r.RequireConfirmedAccount == payload.RequireConfirmedAccount
        && r.CustomAttributes.Count == 1 && r.CustomAttributes[new Identifier("Key")] == "Value"),
      _cancellationToken), Times.Once);

    _languageService.Verify(x => x.SaveAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return null when the realm does not exist.")]
  public async Task Given_NotFound_Then_HandleAsync_Then_NullReturned()
  {
    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = "not-found"
    };
    CreateOrReplaceRealm command = new(Guid.Empty, payload, Version: -1);
    CreateOrReplaceRealmResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.Null(result.Realm);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = "invalid--slug",
      DisplayName = RandomStringGenerator.GetString(999),
      Url = "invalid://url",
      UniqueNameSettings = new UniqueNameSettingsDto(RandomStringGenerator.GetString(999)),
      PasswordSettings = new PasswordSettingsDto(0, 1, false, true, true, true, string.Empty)
    };
    payload.CustomAttributes.Add(new CustomAttribute("123_invalid", string.Empty));

    CreateOrReplaceRealm command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(8, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "SlugValidator" && e.PropertyName == "UniqueSlug");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UrlValidator" && e.PropertyName == "Url");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "UniqueNameSettings.AllowedCharacters");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "PasswordSettings.RequiredLength");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.PropertyName == "PasswordSettings.RequiredUniqueChars");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "PasswordSettings.HashingStrategy");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }

  [Fact(DisplayName = "It should update an existing realm.")]
  public async Task Given_Found_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Secret secret = new(RandomStringGenerator.GetString(999));
    Realm realm = new(new Slug("old-realm"), secret, actorId);
    _realmRepository.Setup(x => x.LoadAsync(realm.Id, _cancellationToken)).ReturnsAsync(realm);

    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = "new-realm",
      DisplayName = " New Realm ",
      Description = "  This is my new realm!  ",
      Url = $"https://www.{_faker.Internet.DomainName()}",
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "Value"));

    RealmDto dto = new();
    _realmQuerier.Setup(x => x.ReadAsync(realm, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceRealm command = new(realm.Id.ToGuid(), payload, Version: null); // TODO(fpion): implement
    CreateOrReplaceRealmResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Realm);
    Assert.Same(dto, result.Realm);

    _realmRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<long?>(), It.IsAny<CancellationToken>()), Times.Never);

    _realmService.Verify(x => x.SaveAsync(
      It.Is<Realm>(r => r.Equals(realm) && r.CreatedBy == actorId && r.UpdatedBy == actorId && r.UniqueSlug.Value == payload.UniqueSlug
        && r.DisplayName != null && r.DisplayName.Value == payload.DisplayName.Trim() && r.Description != null && r.Description.Value == payload.Description.Trim()
        && r.Url != null && r.Url.Value == payload.Url
        && r.UniqueNameSettings.Equals(new Settings.UniqueNameSettings(payload.UniqueNameSettings)) && r.PasswordSettings.Equals(new Settings.PasswordSettings(payload.PasswordSettings))
        && r.RequireUniqueEmail == payload.RequireUniqueEmail && r.RequireConfirmedAccount == payload.RequireConfirmedAccount
        && r.CustomAttributes.Count == 1 && r.CustomAttributes[new Identifier("Key")] == "Value"),
      _cancellationToken), Times.Once);

    _languageService.Verify(x => x.SaveAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);
  }
}
