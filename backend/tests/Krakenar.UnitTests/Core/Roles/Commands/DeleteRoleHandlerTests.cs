using Bogus;
using Krakenar.Core.Realms;
using Krakenar.Core.Roles.Events;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;
using Moq;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteRoleHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IRoleQuerier> _roleQuerier = new();
  private readonly Mock<IRoleRepository> _roleRepository = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly DeleteRoleHandler _handler;

  public DeleteRoleHandlerTests()
  {
    _handler = new(_applicationContext.Object, _roleQuerier.Object, _roleRepository.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "It should delete the role.")]
  public async Task Given_Role_When_HandleAsync_Then_Deleted()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Role role = new(new UniqueName(_uniqueNameSettings, "admin"), actorId, RoleId.NewId(realmId));
    _roleRepository.Setup(x => x.LoadAsync(role.Id, _cancellationToken)).ReturnsAsync(role);

    _userQuerier.Setup(x => x.FindIdsAsync(role.Id, _cancellationToken)).ReturnsAsync([]);

    RoleDto dto = new();
    _roleQuerier.Setup(x => x.ReadAsync(role, _cancellationToken)).ReturnsAsync(dto);

    DeleteRole command = new(role.EntityId);
    RoleDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(role.IsDeleted);
    Assert.Contains(role.Changes, change => change is RoleDeleted deleted && deleted.ActorId == actorId);

    _roleRepository.Verify(x => x.SaveAsync(role, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should remove the role from users.")]
  public async Task Given_UserRoles_When_HandleAsync_Then_Removed()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Role role = new(new UniqueName(_uniqueNameSettings, "admin"));
    _roleRepository.Setup(x => x.LoadAsync(role.Id, _cancellationToken)).ReturnsAsync(role);

    User user1 = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user1.AddRole(role);
    User user2 = new(new UniqueName(_uniqueNameSettings, _faker.Internet.UserName()));
    user2.AddRole(role);
    _userQuerier.Setup(x => x.FindIdsAsync(role.Id, _cancellationToken)).ReturnsAsync([user1.Id, user2.Id]);
    _userRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<UserId>>(y => y.SequenceEqual(new UserId[] { user1.Id, user2.Id })),
      _cancellationToken)).ReturnsAsync([user1, user2]);

    RoleDto dto = new();
    _roleQuerier.Setup(x => x.ReadAsync(role, _cancellationToken)).ReturnsAsync(dto);

    DeleteRole command = new(role.EntityId);
    RoleDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(role.IsDeleted);

    _roleRepository.Verify(x => x.SaveAsync(role, _cancellationToken), Times.Once);

    Assert.Contains(user1.Changes, change => change is UserRoleRemoved removed && removed.ActorId == actorId && removed.RoleId == role.Id);
    Assert.Contains(user2.Changes, change => change is UserRoleRemoved removed && removed.ActorId == actorId && removed.RoleId == role.Id);
    _userRepository.Verify(x => x.SaveAsync(
      It.Is<IEnumerable<User>>(y => y.SequenceEqual(new User[] { user1, user2 })),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the role was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteRole command = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }
}
