using Krakenar.Core.Localization.Events;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using Moq;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetDefaultLanguageHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly SetDefaultLanguageHandler _handler;

  public SetDefaultLanguageHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageQuerier.Object, _languageRepository.Object);
  }

  [Fact(DisplayName = "It should not do anything when the language is already default.")]
  public async Task Given_IsDefault_When_HandleAsync_Then_Nothing()
  {
    Language language = new(new Locale("fr"), isDefault: true);
    language.ClearChanges();
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(dto);

    SetDefaultLanguage command = new(language.EntityId);
    LanguageDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(language.IsDefault);
    Assert.False(language.HasChanges);
    Assert.Empty(language.Changes);

    _languageQuerier.Verify(x => x.FindDefaultIdAsync(It.IsAny<CancellationToken>()), Times.Never);
    _languageRepository.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Language>>(), _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "It should return null when the language was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    SetDefaultLanguage command = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should set the language default.")]
  public async Task Given_NotDefault_When_HandleAsync_Then_SetDefault()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Language @default = new(new Locale("en"), isDefault: true, actorId, LanguageId.NewId(realmId));
    _languageQuerier.Setup(x => x.FindDefaultIdAsync(_cancellationToken)).ReturnsAsync(@default.Id);
    _languageRepository.Setup(x => x.LoadAsync(@default.Id, _cancellationToken)).ReturnsAsync(@default);

    Language language = new(new Locale("fr"), isDefault: false, actorId, LanguageId.NewId(realmId));
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(dto);

    SetDefaultLanguage command = new(language.EntityId);
    LanguageDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.False(@default.IsDefault);
    Assert.Contains(@default.Changes, change => change is LanguageSetDefault setDefault && !setDefault.IsDefault && setDefault.ActorId == actorId);

    Assert.True(language.IsDefault);
    Assert.Contains(language.Changes, change => change is LanguageSetDefault setDefault && setDefault.IsDefault && setDefault.ActorId == actorId);

    _languageRepository.Verify(x => x.SaveAsync(
      It.Is<IEnumerable<Language>>(y => y.SequenceEqual(new Language[] { @default, language })),
      _cancellationToken), Times.Once);
  }
}
