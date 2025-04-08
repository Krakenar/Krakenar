using Moq;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadRoleHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRoleQuerier> _roleQuerier = new();

  private readonly ReadRoleHandler _handler;

  private readonly RoleDto _role1 = new()
  {
    Id = Guid.NewGuid(),
    UniqueName = "admin"
  };
  private readonly RoleDto _role2 = new()
  {
    Id = Guid.NewGuid(),
    UniqueName = "editor"
  };

  public ReadRoleHandlerTests()
  {
    _handler = new(_roleQuerier.Object);

    _roleQuerier.Setup(x => x.ReadAsync(_role1.Id, _cancellationToken)).ReturnsAsync(_role1);
    _roleQuerier.Setup(x => x.ReadAsync(_role2.Id, _cancellationToken)).ReturnsAsync(_role2);
    _roleQuerier.Setup(x => x.ReadAsync(_role1.UniqueName, _cancellationToken)).ReturnsAsync(_role1);
    _roleQuerier.Setup(x => x.ReadAsync(_role2.UniqueName, _cancellationToken)).ReturnsAsync(_role2);
  }

  [Fact(DisplayName = "It should return null when no role was found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadRole query = new(Guid.Empty, "not_found");
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the role found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_RoleReturned()
  {
    ReadRole query = new(_role1.Id, "not_found");
    RoleDto? role = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(role);
    Assert.Same(_role1, role);

    Assert.NotNull(query.UniqueName);
    _roleQuerier.Verify(x => x.ReadAsync(query.UniqueName, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the role found by unique name.")]
  public async Task Given_FoundByUniqueName_When_HandleAsync_Then_RoleReturned()
  {
    ReadRole query = new(Guid.Empty, _role1.UniqueName);
    RoleDto? role = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(role);
    Assert.Same(_role1, role);

    Assert.True(query.Id.HasValue);
    _roleQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple roles were found.")]
  public async Task Given_MultipleFound_When_HandleAsync_Then_TooManyResultsException()
  {
    ReadRole query = new(_role1.Id, _role2.UniqueName);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<RoleDto>>(async () => await _handler.HandleAsync(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
