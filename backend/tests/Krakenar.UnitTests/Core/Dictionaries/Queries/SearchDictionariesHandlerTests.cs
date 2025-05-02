using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Search;
using Moq;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchDictionariesHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IDictionaryQuerier> _dictionaryQuerier = new();

  private readonly SearchDictionariesHandler _handler;

  public SearchDictionariesHandlerTests()
  {
    _handler = new(_dictionaryQuerier.Object);
  }

  [Fact(DisplayName = "It should search dictionarys.")]
  public async Task Given_Payload_When_HandleAsync_Then_Searched()
  {
    SearchDictionariesPayload payload = new();
    SearchResults<DictionaryDto> expected = new();
    _dictionaryQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(expected);

    SearchDictionaries query = new(payload);
    SearchResults<DictionaryDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(expected, results);
  }
}
