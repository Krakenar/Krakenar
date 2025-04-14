using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Moq;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchRealmsHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRealmQuerier> _realmQuerier = new();

  private readonly SearchRealmsHandler _handler;

  public SearchRealmsHandlerTests()
  {
    _handler = new(_realmQuerier.Object);
  }

  [Fact(DisplayName = "It should search realms.")]
  public async Task Given_Payload_When_HandleAsync_Then_Searched()
  {
    SearchRealmsPayload payload = new();
    SearchResults<RealmDto> expected = new();
    _realmQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(expected);

    SearchRealms query = new(payload);
    SearchResults<RealmDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(expected, results);
  }
}
