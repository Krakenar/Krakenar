using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Moq;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchUsersHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IUserQuerier> _userQuerier = new();

  private readonly SearchUsersHandler _handler;

  public SearchUsersHandlerTests()
  {
    _handler = new(_userQuerier.Object);
  }

  [Fact(DisplayName = "It should search users.")]
  public async Task Given_Payload_When_HandleAsync_Then_Searched()
  {
    SearchUsersPayload payload = new();
    SearchResults<UserDto> expected = new();
    _userQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(expected);

    SearchUsers query = new(payload);
    SearchResults<UserDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(expected, results);
  }
}
