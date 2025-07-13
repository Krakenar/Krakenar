using Bogus;
using Krakenar.Contracts.Users;
using Krakenar.Core.Authentication;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Events;
using Logitar;
using Logitar.EventSourcing;
using Moq;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class AuthenticateUserHandlerTests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IAuthenticationService> _authenticationService = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserManager> _userManager = new();

  private readonly AuthenticateUserHandler _handler;

  public AuthenticateUserHandlerTests()
  {
    _handler = new(_applicationContext.Object, _authenticationService.Object, _userManager.Object, _userQuerier.Object);
  }

  [Theory(DisplayName = "It should authenticate the user.")]
  [InlineData(null)]
  [InlineData("175a6011-4e13-405c-a98f-707e0e094f91")]
  public async Task Given_Valid_When_HandleAsync_Then_Authenticated(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new(actorIdValue);
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    UniqueName uniqueName = new(new UniqueNameSettings(), _faker.Person.UserName);
    Base64Password password = new(PasswordString);
    User user = new(uniqueName, password, actorId, UserId.NewId(realmId));
    _userManager.Setup(x => x.FindAsync(user.UniqueName.Value, _cancellationToken)).ReturnsAsync(new FoundUsers(null, user, null, null));

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    AuthenticateUserPayload payload = new(user.UniqueName.Value, PasswordString);
    AuthenticateUser command = new(payload);
    UserDto result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Same(dto, result);

    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, user.AuthenticatedOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(1));
    Assert.Contains(user.Changes, change => change is UserAuthenticated authenticated && authenticated.ActorId == (actorId ?? new ActorId(user.Id.Value)));

    _authenticationService.Verify(x => x.AuthenticatedAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_UserNotFoundException()
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    _userManager.Setup(x => x.FindAsync(It.IsAny<string>(), _cancellationToken)).ReturnsAsync(new FoundUsers(null, null, null, null));

    AuthenticateUserPayload payload = new(_faker.Person.UserName, PasswordString);
    AuthenticateUser command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(realmId.ToGuid(), exception.RealmId);
    Assert.Equal(payload.User, exception.User);
    Assert.Equal("User", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    AuthenticateUserPayload payload = new();
    AuthenticateUser command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "User");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password");
  }
}
