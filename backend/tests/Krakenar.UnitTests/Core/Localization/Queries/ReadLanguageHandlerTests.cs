using Krakenar.Contracts;
using Moq;
using LanguageDto = Krakenar.Contracts.Localization.Language;
using LocaleDto = Krakenar.Contracts.Localization.Locale;

namespace Krakenar.Core.Localization.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadLanguageHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();

  private readonly ReadLanguageHandler _handler;

  private readonly LanguageDto _language1 = new()
  {
    Id = Guid.NewGuid(),
    IsDefault = true,
    Locale = new LocaleDto("en")
  };
  private readonly LanguageDto _language2 = new()
  {
    Id = Guid.NewGuid(),
    Locale = new LocaleDto("fr")
  };
  private readonly LanguageDto _language3 = new()
  {
    Id = Guid.NewGuid(),
    Locale = new LocaleDto("es")
  };

  public ReadLanguageHandlerTests()
  {
    _handler = new(_languageQuerier.Object);

    _languageQuerier.Setup(x => x.ReadAsync(_language1.Id, _cancellationToken)).ReturnsAsync(_language1);
    _languageQuerier.Setup(x => x.ReadAsync(_language2.Id, _cancellationToken)).ReturnsAsync(_language2);
    _languageQuerier.Setup(x => x.ReadAsync(_language3.Id, _cancellationToken)).ReturnsAsync(_language3);
    _languageQuerier.Setup(x => x.ReadAsync(_language1.Locale.Code, _cancellationToken)).ReturnsAsync(_language1);
    _languageQuerier.Setup(x => x.ReadAsync(_language2.Locale.Code, _cancellationToken)).ReturnsAsync(_language2);
    _languageQuerier.Setup(x => x.ReadAsync(_language3.Locale.Code, _cancellationToken)).ReturnsAsync(_language3);
    _languageQuerier.Setup(x => x.ReadDefaultAsync(_cancellationToken)).ReturnsAsync(_language1);
  }

  [Fact(DisplayName = "It should return null when no language was found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadLanguage query = new(Guid.Empty, "not_found", IsDefault: false);
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the default language.")]
  public async Task Given_Default_When_HandleAsync_Then_LanguageReturned()
  {
    ReadLanguage query = new(Guid.Empty, "not_found", IsDefault: true);
    LanguageDto? language = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(language);
    Assert.Same(_language1, language);

    Assert.True(query.Id.HasValue);
    Assert.NotNull(query.Locale);
    _languageQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
    _languageQuerier.Verify(x => x.ReadAsync(query.Locale, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the language found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_LanguageReturned()
  {
    ReadLanguage query = new(_language1.Id, "not_found", IsDefault: false);
    LanguageDto? language = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(language);
    Assert.Same(_language1, language);

    Assert.NotNull(query.Locale);
    _languageQuerier.Verify(x => x.ReadAsync(query.Locale, _cancellationToken), Times.Once);
    _languageQuerier.Verify(x => x.ReadDefaultAsync(It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return the language found by locale.")]
  public async Task Given_FoundByLocale_When_HandleAsync_Then_LanguageReturned()
  {
    ReadLanguage query = new(Guid.Empty, _language1.Locale.Code, IsDefault: false);
    LanguageDto? language = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(language);
    Assert.Same(_language1, language);

    Assert.True(query.Id.HasValue);
    _languageQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
    _languageQuerier.Verify(x => x.ReadDefaultAsync(It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple languages were found.")]
  public async Task Given_MultipleFound_When_HandleAsync_Then_TooManyResultsException()
  {
    ReadLanguage query = new(_language3.Id, _language2.Locale.Code, IsDefault: true);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<LanguageDto>>(async () => await _handler.HandleAsync(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }
}
