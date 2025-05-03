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
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<IRealmManager> _realmManager = new();
  private readonly Mock<IRealmQuerier> _realmQuerier = new();
  private readonly Mock<IRealmRepository> _realmRepository = new();
  private readonly Mock<ISecretManager> _secretManager = new();

  private readonly CreateOrReplaceRealmHandler _handler;

  public CreateOrReplaceRealmHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageQuerier.Object, _languageRepository.Object, _realmManager.Object, _realmQuerier.Object, _realmRepository.Object, _secretManager.Object);
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
    _secretManager.Setup(x => x.Generate(It.Is<RealmId?>(r => r.HasValue))).Returns(secret);

    Locale locale = new("en");
    _languageQuerier.Setup(x => x.FindPlatformDefaultLocaleAsync(_cancellationToken)).ReturnsAsync(locale);

    RealmDto dto = new();
    _realmQuerier.Setup(x => x.ReadAsync(It.IsAny<Realm>(), _cancellationToken)).ReturnsAsync(dto);

    Realm? realm = null;
    _realmManager.Setup(x => x.SaveAsync(It.IsAny<Realm>(), _cancellationToken)).Callback<Realm, CancellationToken>((r, _) => realm = r);

    Language? language = null;
    _languageRepository.Setup(x => x.SaveAsync(It.IsAny<Language>(), _cancellationToken)).Callback<Language, CancellationToken>((l, _) => language = l);

    CreateOrReplaceRealm command = new(id, payload, Version: null);
    CreateOrReplaceRealmResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.Realm);
    Assert.Same(dto, result.Realm);

    Assert.NotNull(realm);
    Assert.Equal(actorId, realm.CreatedBy);
    Assert.Equal(actorId, realm.UpdatedBy);
    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug.Value);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), realm.Description?.Value);
    Assert.Equal(secret, realm.Secret);
    Assert.Equal(payload.Url.Trim(), realm.Url?.Value);
    Assert.Equal(new Settings.UniqueNameSettings(payload.UniqueNameSettings), realm.UniqueNameSettings);
    Assert.Equal(new Settings.PasswordSettings(payload.PasswordSettings), realm.PasswordSettings);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.CustomAttributes.Count, realm.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, realm.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    Assert.NotNull(language);
    Assert.Equal(actorId, language.CreatedBy);
    Assert.Equal(actorId, language.UpdatedBy);
    Assert.Equal(realm.Id, language.RealmId);
    Assert.NotEqual(Guid.Empty, language.EntityId);
    Assert.True(language.IsDefault);
    Assert.Equal(locale, language.Locale);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, realm.Id.ToGuid());

      _realmRepository.Verify(x => x.LoadAsync(It.Is<RealmId>(i => i.ToGuid() == id.Value), _cancellationToken), Times.Once);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, realm.Id.ToGuid());
    }
  }

  [Fact(DisplayName = "It should replace an existing realm.")]
  public async Task Given_NoVersion_When_HandleAsync_Then_Replaced()
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

    Assert.Equal(actorId, realm.UpdatedBy);
    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug.Value);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName?.Value);
    Assert.Equal(payload.Description.Trim(), realm.Description?.Value);
    Assert.Equal(secret, realm.Secret);
    Assert.Equal(payload.Url.Trim(), realm.Url?.Value);
    Assert.Equal(new Settings.UniqueNameSettings(payload.UniqueNameSettings), realm.UniqueNameSettings);
    Assert.Equal(new Settings.PasswordSettings(payload.PasswordSettings), realm.PasswordSettings);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.CustomAttributes.Count, realm.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, realm.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    _realmRepository.Verify(x => x.LoadAsync(It.IsAny<RealmId>(), It.IsAny<long?>(), It.IsAny<CancellationToken>()), Times.Never);
    _realmManager.Verify(x => x.SaveAsync(realm, _cancellationToken), Times.Once);
    _languageRepository.Verify(x => x.SaveAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);
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
  public async Task Given_Version_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Secret secret = new(RandomStringGenerator.GetString(999));
    Realm realm = new(new Slug("old-realm"), secret, actorId);
    _realmRepository.Setup(x => x.LoadAsync(realm.Id, _cancellationToken)).ReturnsAsync(realm);

    Realm reference = new(realm.UniqueSlug, realm.Secret, actorId, realm.Id);
    _realmRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Description description = new("  This is my new realm!  ");
    realm.Description = description;
    realm.Update(actorId);

    CreateOrReplaceRealmPayload payload = new()
    {
      UniqueSlug = "new-realm",
      DisplayName = " New Realm ",
      Url = $"https://www.{_faker.Internet.DomainName()}",
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "Value"));

    RealmDto dto = new();
    _realmQuerier.Setup(x => x.ReadAsync(realm, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceRealm command = new(realm.Id.ToGuid(), payload, reference.Version);
    CreateOrReplaceRealmResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Realm);
    Assert.Same(dto, result.Realm);

    Assert.Equal(actorId, realm.UpdatedBy);
    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug.Value);
    Assert.Equal(payload.DisplayName.Trim(), realm.DisplayName?.Value);
    Assert.Equal(description, realm.Description);
    Assert.Equal(secret, realm.Secret);
    Assert.Equal(payload.Url.Trim(), realm.Url?.Value);
    Assert.Equal(new Settings.UniqueNameSettings(payload.UniqueNameSettings), realm.UniqueNameSettings);
    Assert.Equal(new Settings.PasswordSettings(payload.PasswordSettings), realm.PasswordSettings);
    Assert.Equal(payload.RequireUniqueEmail, realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);
    Assert.Equal(payload.CustomAttributes.Count, realm.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, realm.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    _realmRepository.Verify(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken), Times.Once);
    _realmManager.Verify(x => x.SaveAsync(realm, _cancellationToken), Times.Once);
    _languageRepository.Verify(x => x.SaveAsync(It.IsAny<Language>(), It.IsAny<CancellationToken>()), Times.Never);
  }
}
