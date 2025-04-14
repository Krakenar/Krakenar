using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Moq;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchLanguagesHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ILanguageQuerier> _languageQuerier = new();

  private readonly SearchLanguagesHandler _handler;

  public SearchLanguagesHandlerTests()
  {
    _handler = new(_languageQuerier.Object);
  }

  [Fact(DisplayName = "It should search languages.")]
  public async Task Given_Payload_When_HandleAsync_Then_Searched()
  {
    SearchLanguagesPayload payload = new();
    SearchResults<LanguageDto> expected = new();
    _languageQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(expected);

    SearchLanguages query = new(payload);
    SearchResults<LanguageDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(expected, results);
  }
}
