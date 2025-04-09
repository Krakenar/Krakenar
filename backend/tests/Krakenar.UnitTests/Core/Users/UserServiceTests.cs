using Bogus;
using Krakenar.Core.Settings;
using Moq;

namespace Krakenar.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class UserServiceTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly UserService _service;

  public UserServiceTests()
  {
    _service = new(_applicationContext.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "SaveAsync: it should save the user when the unique name has not changed.")]
  public async Task Given_UniqueNameNotChanged_When_SaveAsync_Then_Saved()
  {
    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.ClearChanges();

    user.Delete();
    await _service.SaveAsync(user, _cancellationToken);

    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the user when there is no unique name conflict.")]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved()
  {
    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    _userQuerier.Setup(x => x.FindIdAsync(user.UniqueName, _cancellationToken)).ReturnsAsync(user.Id);

    await _service.SaveAsync(user, _cancellationToken);

    _userQuerier.Verify(x => x.FindIdAsync(user.UniqueName, _cancellationToken), Times.Once);
    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should throw UniqueNameAlreadyUsedException when there is a unique name conflict.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueNameAlreadyUsedException()
  {
    User conflict = new(new UniqueName(_uniqueNameSettings, _faker.Internet.UserName()));
    _userQuerier.Setup(x => x.FindIdAsync(conflict.UniqueName, _cancellationToken)).ReturnsAsync(conflict.Id);

    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.ClearChanges();
    user.SetUniqueName(conflict.UniqueName);

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _service.SaveAsync(user, _cancellationToken));
    Assert.Equal(user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("User", exception.EntityType);
    Assert.Equal(user.EntityId, exception.EntityId);
    Assert.Equal(conflict.EntityId, exception.ConflictId);
    Assert.Equal(user.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
