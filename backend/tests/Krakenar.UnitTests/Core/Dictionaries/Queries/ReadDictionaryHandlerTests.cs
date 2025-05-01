using Krakenar.Contracts;
using Krakenar.Contracts.Localization;
using Moq;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadDictionaryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IDictionaryQuerier> _dictionaryQuerier = new();

  private readonly ReadDictionaryHandler _handler;

  private readonly DictionaryDto _dictionary1 = new()
  {
    Id = Guid.NewGuid(),
    Language = new Language
    {
      Id = Guid.NewGuid()
    }
  };
  private readonly DictionaryDto _dictionary2 = new()
  {
    Id = Guid.NewGuid(),
    Language = new Language
    {
      Id = Guid.NewGuid()
    }
  };

  public ReadDictionaryHandlerTests()
  {
    _handler = new(_dictionaryQuerier.Object);

    _dictionaryQuerier.Setup(x => x.ReadAsync(_dictionary1.Id, _cancellationToken)).ReturnsAsync(_dictionary1);
    _dictionaryQuerier.Setup(x => x.ReadAsync(_dictionary2.Id, _cancellationToken)).ReturnsAsync(_dictionary2);
    _dictionaryQuerier.Setup(x => x.ReadByLanguageAsync(_dictionary1.Language.Id, _cancellationToken)).ReturnsAsync(_dictionary1);
    _dictionaryQuerier.Setup(x => x.ReadByLanguageAsync(_dictionary2.Language.Id, _cancellationToken)).ReturnsAsync(_dictionary2);
  }

  [Fact(DisplayName = "It should return null when no dictionary was found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadDictionary query = new(Guid.Empty, Guid.Empty);
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the dictionary found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_DictionaryReturned()
  {
    ReadDictionary query = new(_dictionary1.Id, Guid.Empty);
    DictionaryDto? dictionary = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(dictionary);
    Assert.Same(_dictionary1, dictionary);

    Assert.NotNull(query.LanguageId);
    _dictionaryQuerier.Verify(x => x.ReadByLanguageAsync(query.LanguageId.Value, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the dictionary found by language ID.")]
  public async Task Given_FoundByLanguageId_When_HandleAsync_Then_DictionaryReturned()
  {
    ReadDictionary query = new(Guid.Empty, _dictionary1.Language.Id);
    DictionaryDto? dictionary = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(dictionary);
    Assert.Same(_dictionary1, dictionary);

    Assert.True(query.Id.HasValue);
    _dictionaryQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple dictionarys were found.")]
  public async Task Given_MultipleFound_When_HandleAsync_Then_TooManyResultsException()
  {
    ReadDictionary query = new(_dictionary1.Id, _dictionary2.Language.Id);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<DictionaryDto>>(async () => await _handler.HandleAsync(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
