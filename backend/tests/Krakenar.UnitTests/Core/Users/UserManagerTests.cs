using Bogus;
using Krakenar.Contracts.Users;
using Krakenar.Core.Settings;
using Moq;

namespace Krakenar.Core.Users;

[Trait(Traits.Category, Categories.Unit)]
public class UserManagerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();
  private readonly UniqueNameSettings _uniqueNameSettings = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly UserManager _manager;

  public UserManagerTests()
  {
    _manager = new(_applicationContext.Object, _userQuerier.Object, _userRepository.Object);

    _applicationContext.SetupGet(x => x.RequireUniqueEmail).Returns(true);
  }

  [Fact(DisplayName = "SaveAsync: it should save the user when no unique data changed.")]
  public async Task Given_NoUniqueDataChange_When_SaveAsync_Then_Saved()
  {
    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.ClearChanges();

    user.Delete();
    await _manager.SaveAsync(user, _cancellationToken);

    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never);
    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the user when there is no custom identifier conflict.")]
  public async Task Given_NoCustomIdentifierConflict_When_SaveAsync_Then_Saved()
  {
    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.ClearChanges();

    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");
    user.SetCustomIdentifier(key, value);
    _userQuerier.Setup(x => x.FindIdAsync(key, value, _cancellationToken)).ReturnsAsync(user.Id);

    await _manager.SaveAsync(user, _cancellationToken);

    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdAsync(key, value, _cancellationToken), Times.Once);
    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the user when the email addresses are not unique.")]
  public async Task Given_EmailNotUnique_When_SaveAsync_Then_Saved()
  {
    _applicationContext.SetupGet(x => x.RequireUniqueEmail).Returns(false);

    Email email = new(_faker.Person.Email);

    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.ClearChanges();
    user.SetEmail(email);

    await _manager.SaveAsync(user, _cancellationToken);

    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never);
    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the user when the email address is now null.")]
  public async Task Given_NullEmail_When_SaveAsync_Then_Saved()
  {
    Email email = new(_faker.Person.Email);

    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.SetEmail(email);
    user.ClearChanges();
    user.SetEmail(null);

    await _manager.SaveAsync(user, _cancellationToken);

    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never);
    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Theory(DisplayName = "SaveAsync: it should save the user when there is no email address conflict.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NoEmailAddressConflict_When_SaveAsync_Then_Saved(bool found)
  {
    Email email = new(_faker.Person.Email);

    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.ClearChanges();
    user.SetEmail(email);

    _userQuerier.Setup(x => x.FindIdsAsync(email, _cancellationToken)).ReturnsAsync(found ? [user.Id] : []);

    await _manager.SaveAsync(user, _cancellationToken);

    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<UniqueName>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdsAsync(email, _cancellationToken), Times.Once);
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never);
    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the user when there is no unique name conflict.")]
  public async Task Given_NoUniqueNameConflict_When_SaveAsync_Then_Saved()
  {
    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    _userQuerier.Setup(x => x.FindIdAsync(user.UniqueName, _cancellationToken)).ReturnsAsync(user.Id);

    await _manager.SaveAsync(user, _cancellationToken);

    _userQuerier.Verify(x => x.FindIdAsync(user.UniqueName, _cancellationToken), Times.Once);
    _userQuerier.Verify(x => x.FindIdsAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.FindIdAsync(It.IsAny<Identifier>(), It.IsAny<CustomIdentifier>(), It.IsAny<CancellationToken>()), Times.Never);
    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should throw CustomIdentifierAlreadyUsedException when there is a custom identifier conflict.")]
  public async Task Given_CustomIdentifierConflict_When_SaveAsync_Then_CustomIdentifierAlreadyUsedException()
  {
    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");

    User conflict = new(new UniqueName(_uniqueNameSettings, _faker.Internet.UserName()));
    conflict.SetCustomIdentifier(key, value);
    _userQuerier.Setup(x => x.FindIdAsync(key, value, _cancellationToken)).ReturnsAsync(conflict.Id);

    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.ClearChanges();
    user.SetCustomIdentifier(key, value);

    var exception = await Assert.ThrowsAsync<CustomIdentifierAlreadyUsedException>(async () => await _manager.SaveAsync(user, _cancellationToken));
    Assert.Equal(user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("User", exception.EntityType);
    Assert.Equal(user.EntityId, exception.EntityId);
    Assert.Equal(conflict.EntityId, exception.ConflictId);
    Assert.Equal(key.Value, exception.Key);
    Assert.Equal(value.Value, exception.Value);
  }

  [Fact(DisplayName = "SaveAsync: it should throw EmailAddressAlreadyUsedException when there is an email address conflict.")]
  public async Task Given_EmailAddressConflict_When_SaveAsync_Then_EmailAddressAlreadyUsedException()
  {
    Email email = new(_faker.Person.Email);

    User conflict = new(new UniqueName(_uniqueNameSettings, _faker.Internet.UserName()));
    conflict.SetEmail(email);
    _userQuerier.Setup(x => x.FindIdsAsync(email, _cancellationToken)).ReturnsAsync([conflict.Id]);

    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.ClearChanges();
    user.SetEmail(email);

    var exception = await Assert.ThrowsAsync<EmailAddressAlreadyUsedException>(async () => await _manager.SaveAsync(user, _cancellationToken));
    Assert.Equal(user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(user.EntityId, exception.UserId);
    Assert.Equal(conflict.EntityId, Assert.Single(exception.ConflictIds));
    Assert.Equal(email.Address, exception.EmailAddress);
    Assert.Equal("Email", exception.PropertyName);
  }

  [Fact(DisplayName = "SaveAsync: it should throw UniqueNameAlreadyUsedException when there is a unique name conflict.")]
  public async Task Given_UniqueNameConflict_When_SaveAsync_Then_UniqueNameAlreadyUsedException()
  {
    User conflict = new(new UniqueName(_uniqueNameSettings, _faker.Internet.UserName()));
    _userQuerier.Setup(x => x.FindIdAsync(conflict.UniqueName, _cancellationToken)).ReturnsAsync(conflict.Id);

    User user = new(new UniqueName(_uniqueNameSettings, _faker.Person.UserName));
    user.ClearChanges();
    user.SetUniqueName(conflict.UniqueName);

    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _manager.SaveAsync(user, _cancellationToken));
    Assert.Equal(user.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("User", exception.EntityType);
    Assert.Equal(user.EntityId, exception.EntityId);
    Assert.Equal(conflict.EntityId, exception.ConflictId);
    Assert.Equal(user.UniqueName.Value, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
