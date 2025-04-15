using Bogus;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;
using Moq;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class RemoveUserIdentifierHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly RemoveUserIdentifierHandler _handler;

  public RemoveUserIdentifierHandlerTests()
  {
    _handler = new(_applicationContext.Object, _userQuerier.Object, _userRepository.Object);

    _applicationContext.SetupGet(x => x.PasswordSettings).Returns(new PasswordSettings());
  }

  [Theory(DisplayName = "It should remove a user identifier.")]
  [InlineData(null, false)]
  [InlineData("b417b9f1-824e-469e-adba-70bcd5baf509", true)]
  public async Task Given_UserFound_When_HandleAsync_Then_IdentifierRemoved(string? actorIdValue, bool exists)
  {
    ActorId? actorId = actorIdValue is null ? null : new(actorIdValue);
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), password: null, actorId, UserId.NewId(realmId));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    Identifier key = new("Google");
    if (exists)
    {
      user.SetCustomIdentifier(key, new CustomIdentifier("1234567890"), actorId);
    }
    user.ClearChanges();

    RemoveUserIdentifier command = new(user.EntityId, key.Value);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    if (exists)
    {
      Assert.Contains(user.Changes, change => change is UserIdentifierRemoved removed && removed.ActorId == actorId && removed.Key.Value == command.Key);
    }
    else
    {
      Assert.False(user.HasChanges);
      Assert.Empty(user.Changes);
    }

    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the user was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    RemoveUserIdentifier command = new(Guid.Empty, "Google");
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    RemoveUserIdentifier command = new(Guid.Empty, "123_invalid");
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "Key");
  }
}
