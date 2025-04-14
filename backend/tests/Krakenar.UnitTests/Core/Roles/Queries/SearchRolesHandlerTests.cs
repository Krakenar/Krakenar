using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Moq;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchRolesHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRoleQuerier> _roleQuerier = new();

  private readonly SearchRolesHandler _handler;

  public SearchRolesHandlerTests()
  {
    _handler = new(_roleQuerier.Object);
  }

  [Fact(DisplayName = "It should search roles.")]
  public async Task Given_Payload_When_HandleAsync_Then_Searched()
  {
    SearchRolesPayload payload = new();
    SearchResults<RoleDto> expected = new();
    _roleQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(expected);

    SearchRoles query = new(payload);
    SearchResults<RoleDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(expected, results);
  }
}
