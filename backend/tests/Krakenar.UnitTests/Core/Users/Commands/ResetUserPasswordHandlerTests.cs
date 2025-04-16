using Bogus;
using Krakenar.Contracts.Users;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;
using Moq;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class ResetUserPasswordHandlerTests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IPasswordService> _passwordService = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly ResetUserPasswordHandler _handler;

  public ResetUserPasswordHandlerTests()
  {
    _handler = new(_applicationContext.Object, _passwordService.Object, _userQuerier.Object, _userRepository.Object);

    _applicationContext.SetupGet(x => x.PasswordSettings).Returns(new PasswordSettings());
  }

  [Theory(DisplayName = "It should reset the user password.")]
  [InlineData(null)]
  [InlineData("a769d8c8-e0f0-461b-8ce0-9a2cce85a041")]
  public async Task Given_UserFound_When_HandleAsync_Then_PasswordReset(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new(actorIdValue);
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), password: null, actorId, UserId.NewId(realmId));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    Base64Password password = new(PasswordString);
    _passwordService.Setup(x => x.ValidateAndHash(PasswordString, null)).Returns(password);

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    ResetUserPasswordPayload payload = new(PasswordString);
    ResetUserPassword command = new(user.EntityId, payload);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(user.HasPassword);
    Assert.Contains(user.Changes, change => change is UserPasswordReset reset
      && reset.ActorId == (actorId ?? new ActorId(user.Id.Value))
      && reset.Password == password);

    _userRepository.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the user was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ResetUserPasswordPayload payload = new(PasswordString);
    ResetUserPassword command = new(Guid.Empty, payload);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    ResetUserPasswordPayload payload = new();
    ResetUserPassword command = new(Guid.Empty, payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(7, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordTooShort" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUniqueChars" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresNonAlphanumeric" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresLower" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresUpper" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "PasswordRequiresDigit" && e.PropertyName == "Password");
  }
}
