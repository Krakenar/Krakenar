using Bogus;
using Krakenar.Contracts.Users;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SaveUserIdentifierHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly SaveUserIdentifierHandler _handler;

  public SaveUserIdentifierHandlerTests()
  {
    _handler = new(_applicationContext.Object, _userQuerier.Object, _userRepository.Object, _userService.Object);

    _applicationContext.SetupGet(x => x.PasswordSettings).Returns(new PasswordSettings());
  }

  [Theory(DisplayName = "It should save a user identifier.")]
  [InlineData(null)]
  [InlineData("d135ef4e-0996-4327-b372-5f2f72b18c1d")]
  public async Task Given_UserFound_When_HandleAsync_Then_IdentifierSaved(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new(actorIdValue);
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), password: null, actorId, UserId.NewId(realmId));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    SaveUserIdentifierPayload payload = new(" 1234567890 ");
    SaveUserIdentifier command = new(user.EntityId, "Google", payload);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.Contains(user.Changes, change => change is UserIdentifierChanged changed && changed.ActorId == actorId
      && changed.Key.Value == command.Key && changed.Value.Value == payload.Value.Trim());

    _userService.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the user was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    SaveUserIdentifierPayload payload = new(Guid.NewGuid().ToString());
    SaveUserIdentifier command = new(Guid.Empty, "Google", payload);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    SaveUserIdentifierPayload payload = new(RandomStringGenerator.GetString(999));
    SaveUserIdentifier command = new(Guid.Empty, "123_invalid", payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "Key");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "MaximumLengthValidator" && e.PropertyName == "Value");
  }
}
