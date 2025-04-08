using Moq;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadSessionHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISessionQuerier> _sessionQuerier = new();

  private readonly ReadSessionHandler _handler;

  private readonly SessionDto _session = new()
  {
    Id = Guid.NewGuid()
  };

  public ReadSessionHandlerTests()
  {
    _handler = new(_sessionQuerier.Object);

    _sessionQuerier.Setup(x => x.ReadAsync(_session.Id, _cancellationToken)).ReturnsAsync(_session);
  }

  [Fact(DisplayName = "It should return null when the session was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadSession query = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the session found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_SessionReturned()
  {
    ReadSession query = new(_session.Id);
    SessionDto? session = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(session);
    Assert.Same(_session, session);
  }
}
