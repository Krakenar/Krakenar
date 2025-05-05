using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Search;
using Moq;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;

namespace Krakenar.Core.ApiKeys.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchApiKeysHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApiKeyQuerier> _apikeyQuerier = new();

  private readonly SearchApiKeysHandler _handler;

  public SearchApiKeysHandlerTests()
  {
    _handler = new(_apikeyQuerier.Object);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Payload_When_HandleAsync_Then_ResultsReturned()
  {
    SearchApiKeysPayload payload = new();
    SearchResults<ApiKeyDto> apikeys = new();
    _apikeyQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(apikeys);

    SearchApiKeys query = new(payload);
    SearchResults<ApiKeyDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(apikeys, results);
  }
}
