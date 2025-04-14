using Krakenar.Contracts.Localization;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using Moq;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateOrReplaceLanguageHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();
  private readonly Mock<ILanguageService> _languageService = new();

  private readonly CreateOrReplaceLanguageHandler _handler;

  public CreateOrReplaceLanguageHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageQuerier.Object, _languageRepository.Object, _languageService.Object);

    _applicationContext.SetupGet(x => x.UniqueNameSettings).Returns(_uniqueNameSettings);
  }

  [Theory(DisplayName = "It should create a new language.")]
  [InlineData(null)]
  [InlineData("633e6868-53fb-48f3-ab9f-0ec6eae809f5")]
  public async Task Given_NotExists_When_HandleAsync_Then_Created(string? idValue)
  {
    Guid? id = idValue is null ? null : Guid.Parse(idValue);

    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "fr"
    };

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(It.IsAny<Language>(), _cancellationToken)).ReturnsAsync(dto);

    Language? language = null;
    _languageService.Setup(x => x.SaveAsync(It.IsAny<Language>(), _cancellationToken)).Callback<Language, CancellationToken>((r, _) => language = r);

    CreateOrReplaceLanguage command = new(id, payload, Version: null);
    CreateOrReplaceLanguageResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.True(result.Created);
    Assert.NotNull(result.Language);
    Assert.Same(dto, result.Language);

    Assert.NotNull(language);
    Assert.Equal(actorId, language.CreatedBy);
    Assert.Equal(actorId, language.UpdatedBy);
    Assert.Equal(realmId, language.RealmId);
    Assert.False(language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);

    if (id.HasValue)
    {
      Assert.Equal(id.Value, language.EntityId);

      _languageRepository.Verify(x => x.LoadAsync(It.Is<LanguageId>(i => i.RealmId == realmId && i.EntityId == id.Value), _cancellationToken), Times.Once);
    }
    else
    {
      Assert.NotEqual(Guid.Empty, language.EntityId);
    }
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task Given_Found_When_HandleAsync_Then_Replaced()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Language language = new(new Locale("fr"), isDefault: false, actorId);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "fr-CA"
    };

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceLanguage command = new(language.EntityId, payload, Version: null);
    CreateOrReplaceLanguageResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Language);
    Assert.Same(dto, result.Language);

    Assert.Equal(actorId, language.UpdatedBy);
    Assert.Equal(payload.Locale, language.Locale.Code);

    _languageRepository.Verify(x => x.LoadAsync(It.IsAny<LanguageId>(), It.IsAny<long?>(), It.IsAny<CancellationToken>()), Times.Never);
    _languageService.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the language does not exist.")]
  public async Task Given_NotFound_Then_HandleAsync_Then_NullReturned()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "es"
    };
    CreateOrReplaceLanguage command = new(Guid.Empty, payload, Version: -1);
    CreateOrReplaceLanguageResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.Null(result.Language);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "invalid"
    };

    CreateOrReplaceLanguage command = new(Id: null, payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Locale");
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task Given_Found_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Language language = new(new Locale("en"), isDefault: true, actorId);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    Language reference = new(language.Locale, language.IsDefault, actorId, language.Id);
    _languageRepository.Setup(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken)).ReturnsAsync(reference);

    Locale locale = new("en-CA");
    language.SetLocale(locale, actorId);

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(dto);

    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "en"
    };
    CreateOrReplaceLanguage command = new(language.EntityId, payload, reference.Version);
    CreateOrReplaceLanguageResult result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.False(result.Created);
    Assert.NotNull(result.Language);
    Assert.Same(dto, result.Language);

    Assert.Equal(actorId, language.UpdatedBy);
    Assert.True(language.IsDefault);
    Assert.Equal(locale, language.Locale);

    _languageRepository.Verify(x => x.LoadAsync(reference.Id, reference.Version, _cancellationToken), Times.Once);
    _languageService.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
  }
}
