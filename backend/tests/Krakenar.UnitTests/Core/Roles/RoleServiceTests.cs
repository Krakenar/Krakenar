using Krakenar.Core.Settings;
using Moq;

namespace Krakenar.Core.Roles;

[Trait(Traits.Category, Categories.Unit)]
public class RoleServiceTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Mock<IRoleQuerier> _roleQuerier = new();
  private readonly Mock<IRoleRepository> _roleRepository = new();

  private readonly RoleService _service;

  public RoleServiceTests()
  {
    _service = new(_roleQuerier.Object, _roleRepository.Object);
  }

  [Fact(DisplayName = "SaveAsync: it should save the role when the unique name has not changed.")]
  public async Task Given_UniqueNameNotChanged_When_SaveAsync_Then_Saved()
  {
    Role role = new(new UniqueName(_uniqueNameSettings, "admin"));
    role.ClearChanges();

    role.Delete();
    await _service.SaveAsync(role, _cancellationToken);

    _roleQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _roleRepository.Verify(x => x.SaveAsync(role, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the role when there is no unique name conflict.")]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved()
  {
    Role role = new(new UniqueName(_uniqueNameSettings, "guest"));
    _roleQuerier.Setup(x => x.FindIdAsync(role.UniqueName, _cancellationToken)).ReturnsAsync(role.Id);

    await _service.SaveAsync(role, _cancellationToken);

    _roleQuerier.Verify(x => x.FindIdAsync(role.UniqueName, _cancellationToken), Times.Once);
    _roleRepository.Verify(x => x.SaveAsync(role, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should throw UniqueNameAlreadyUsedException when there is a unique name conflict.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueNameAlreadyUsedException()
  {
    Role conflict = new(new UniqueName(_uniqueNameSettings, "admin"));
    _roleQuerier.Setup(x => x.FindIdAsync(conflict.UniqueName, _cancellationToken)).ReturnsAsync(conflict.Id);

    Role role = new(new UniqueName(_uniqueNameSettings, "guest"));
    role.ClearChanges();
    role.SetUniqueName(conflict.UniqueName);

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _service.SaveAsync(role, _cancellationToken));
    Assert.Equal(role.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("Role", exception.EntityType);
    Assert.Equal(role.EntityId, exception.EntityId);
    Assert.Equal(conflict.EntityId, exception.ConflictId);
    Assert.Equal(role.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
