using Krakenar.Contracts.Localization;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using Moq;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateLanguageHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILanguageManager> _languageManager = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly UpdateLanguageHandler _handler;

  public UpdateLanguageHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageManager.Object, _languageQuerier.Object, _languageRepository.Object);
  }

  [Fact(DisplayName = "It should return null when the language was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    UpdateLanguagePayload payload = new();
    UpdateLanguage command = new(Guid.Empty, payload);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateLanguagePayload payload = new()
    {
      Locale = "invalid"
    };

    UpdateLanguage command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Locale");
  }

  [Fact(DisplayName = "It should update the language.")]
  public async Task Given_Language_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Language language = new(new Locale("en"), isDefault: true, actorId, LanguageId.NewId(realmId));
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    UpdateLanguagePayload payload = new()
    {
      Locale = "en-CA"
    };

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(dto);

    UpdateLanguage command = new(language.EntityId, payload);
    LanguageDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    _languageManager.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);

    Assert.True(language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }
}
