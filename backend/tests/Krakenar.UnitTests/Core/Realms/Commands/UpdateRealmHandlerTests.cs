using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using PasswordSettingsDto = Krakenar.Contracts.Settings.PasswordSettings;
using RealmDto = Krakenar.Contracts.Realms.Realm;
using UniqueNameSettingsDto = Krakenar.Contracts.Settings.UniqueNameSettings;

namespace Krakenar.Core.Realms.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateRealmHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IRealmManager> _realmManager = new();
  private readonly Mock<IRealmQuerier> _realmQuerier = new();
  private readonly Mock<IRealmRepository> _realmRepository = new();

  private readonly UpdateRealmHandler _handler;

  public UpdateRealmHandlerTests()
  {
    _handler = new(_applicationContext.Object, _realmManager.Object, _realmQuerier.Object, _realmRepository.Object);
  }

  [Fact(DisplayName = "It should return null when the realm was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    UpdateRealmPayload payload = new();
    UpdateRealm command = new(Guid.Empty, payload);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateRealmPayload payload = new()
    {
      UniqueSlug = "hello--world!",
      DisplayName = new Contracts.Change<string>(RandomStringGenerator.GetString(999)),
      Url = new Contracts.Change<string>("invalid"),
      UniqueNameSettings = new UniqueNameSettingsDto(allowedCharacters: "    "),
      PasswordSettings = new PasswordSettingsDto(0, 6, false, true, true, true, string.Empty)
    };
    payload.CustomAttributes.Add(new CustomAttribute("123_invalid", string.Empty));

    UpdateRealm command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(8, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "SlugValidator" && e.PropertyName == "UniqueSlug");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "DisplayName.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "UrlValidator" && e.PropertyName == "Url.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "UniqueNameSettings.AllowedCharacters");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "PasswordSettings.RequiredLength");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.PropertyName == "PasswordSettings.RequiredUniqueChars");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "PasswordSettings.HashingStrategy");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }

  [Fact(DisplayName = "It should update the realm.")]
  public async Task Given_Realm_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    Realm realm = new(new Slug("the-realm"), secret, actorId);
    realm.SetCustomAttribute(new Identifier("Key"), "Value");
    realm.SetCustomAttribute(new Identifier("Id"), Guid.NewGuid().ToString());
    realm.Update(actorId);
    _realmRepository.Setup(x => x.LoadAsync(realm.Id, _cancellationToken)).ReturnsAsync(realm);

    UpdateRealmPayload payload = new()
    {
      UniqueSlug = "new-realm",
      DisplayName = new Contracts.Change<string>(" New Realm "),
      Description = new Contracts.Change<string>("  This is the new realm.  "),
      Url = new Contracts.Change<string>($"https://www.{_faker.Internet.DomainName()}"),
      RequireConfirmedAccount = false
    };
    payload.CustomAttributes.Add(new CustomAttribute("Id", "   "));
    string externalId = Guid.NewGuid().ToString();
    payload.CustomAttributes.Add(new CustomAttribute("ExternalId", externalId));

    RealmDto dto = new();
    _realmQuerier.Setup(x => x.ReadAsync(realm, _cancellationToken)).ReturnsAsync(dto);

    UpdateRealm command = new(realm.Id.ToGuid(), payload);
    RealmDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    _realmManager.Verify(x => x.SaveAsync(realm, _cancellationToken), Times.Once);

    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug.Value);
    Assert.Equal(payload.DisplayName.Value?.Trim(), realm.DisplayName?.Value);
    Assert.Equal(payload.Description.Value?.Trim(), realm.Description?.Value);
    Assert.Equal(payload.Url.Value, realm.Url?.Value);
    Assert.Equal(new UniqueNameSettings(), realm.UniqueNameSettings);
    Assert.Equal(new PasswordSettings(), realm.PasswordSettings);
    Assert.True(realm.RequireUniqueEmail);
    Assert.Equal(payload.RequireConfirmedAccount, realm.RequireConfirmedAccount);

    Assert.Equal(2, realm.CustomAttributes.Count);
    Assert.Contains(realm.CustomAttributes, c => c.Key.Value == "Key" && c.Value == "Value");
    Assert.Contains(realm.CustomAttributes, c => c.Key.Value == "ExternalId" && c.Value == externalId);
  }
}
