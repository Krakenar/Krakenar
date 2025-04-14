using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Moq;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchSessionsHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISessionQuerier> _sessionQuerier = new();

  private readonly SearchSessionsHandler _handler;

  public SearchSessionsHandlerTests()
  {
    _handler = new(_sessionQuerier.Object);
  }

  [Fact(DisplayName = "It should search sessions.")]
  public async Task Given_Payload_When_HandleAsync_Then_Searched()
  {
    SearchSessionsPayload payload = new();
    SearchResults<SessionDto> expected = new();
    _sessionQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(expected);

    SearchSessions query = new(payload);
    SearchResults<SessionDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(expected, results);
  }
}
