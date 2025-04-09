using Moq;

namespace Krakenar.Core.Localization;

[Trait(Traits.Category, Categories.Unit)]
public class LanguageServiceTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly LanguageService _service;

  public LanguageServiceTests()
  {
    _service = new(_languageQuerier.Object, _languageRepository.Object);
  }

  [Fact(DisplayName = "SaveAsync: it should save the language when the locale has not changed.")]
  public async Task Given_LocaleNotChanged_When_SaveAsync_Then_Saved()
  {
    Language language = new(new Locale("en"));
    language.ClearChanges();

    language.SetDefault();
    await _service.SaveAsync(language, _cancellationToken);

    _languageQuerier.Verify(x => x.FindIdAsync(It.IsAny<Locale>(), It.IsAny<CancellationToken>()), Times.Never);
    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the language when there is no locale conflict.")]
  public async Task Given_NoLocaleConflict_When_SaveAsync_Then_Saved()
  {
    Language language = new(new Locale("en"));
    _languageQuerier.Setup(x => x.FindIdAsync(language.Locale, _cancellationToken)).ReturnsAsync(language.Id);

    await _service.SaveAsync(language, _cancellationToken);

    _languageQuerier.Verify(x => x.FindIdAsync(language.Locale, _cancellationToken), Times.Once);
    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should throw LocaleAlreadyUsedException when there is a locale conflict.")]
  public async Task Given_LocaleConflict_When_SaveAsync_Then_LocaleAlreadyUsedException()
  {
    Language conflict = new(new Locale("en"));
    _languageQuerier.Setup(x => x.FindIdAsync(conflict.Locale, _cancellationToken)).ReturnsAsync(conflict.Id);

    Language language = new(new Locale("fr"));
    language.ClearChanges();
    language.SetLocale(conflict.Locale);

    var exception = await Assert.ThrowsAsync<LocaleAlreadyUsedException>(async () => await _service.SaveAsync(language, _cancellationToken));
    Assert.Equal(language.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(language.EntityId, exception.LanguageId);
    Assert.Equal(conflict.EntityId, exception.ConflictId);
    Assert.Equal(language.Locale.ToString(), exception.Locale);
    Assert.Equal("Locale", exception.PropertyName);
  }
}
