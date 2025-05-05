using Moq;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadApiKeyHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApiKeyQuerier> _apiKeyQuerier = new();

  private readonly ReadApiKeyHandler _handler;

  public ReadApiKeyHandlerTests()
  {
    _handler = new(_apiKeyQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when the API key could not be found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    Assert.Null(await _handler.HandleAsync(new ReadApiKey(Guid.NewGuid()), _cancellationToken));
  }

  [Fact(DisplayName = "It should return the API key found by ID.")]
  public async Task Given_Found_When_HandleAsync_Then_ApiKeyReturned()
  {
    ApiKeyDto apiKey = new()
    {
      Id = Guid.NewGuid()
    };
    _apiKeyQuerier.Setup(x => x.ReadAsync(apiKey.Id, _cancellationToken)).ReturnsAsync(apiKey);

    ReadApiKey query = new(apiKey.Id);
    ApiKeyDto? result = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(result);
    Assert.Same(apiKey, result);
  }
}
