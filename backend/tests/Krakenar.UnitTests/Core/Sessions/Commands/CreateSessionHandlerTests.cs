using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class CreateSessionHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IPasswordService> _passwordService = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly CreateSessionHandler _handler;

  private readonly User _user;

  public CreateSessionHandlerTests()
  {
    _handler = new(_applicationContext.Object, _passwordService.Object, _sessionQuerier.Object, _sessionRepository.Object, _userService.Object);

    UniqueName uniqueName = new(new UniqueNameSettings(), _faker.Person.UserName);
    _user = new User(uniqueName);
    _userService.Setup(x => x.FindAsync(_user.UniqueName.Value, _cancellationToken)).ReturnsAsync(new FoundUsers(null, _user, null, null));
  }

  [Fact(DisplayName = "It should create a new ephemereal session.")]
  public async Task Given_Ephemereal_When_HandleAsync_Then_SessionCreated()
  {
    SessionDto session = new();
    _sessionQuerier.Setup(x => x.ReadAsync(It.IsAny<Session>(), _cancellationToken)).ReturnsAsync(session);

    CreateSessionPayload payload = new(_user.UniqueName.Value);
    CreateSession command = new(payload);
    SessionDto result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Same(session, result);
    Assert.Null(result.RefreshToken);

    Assert.Contains(_user.Changes, change => change is UserSignedIn signedIn && signedIn.ActorId?.Value == _user.Id.Value);

    _userService.Verify(x => x.SaveAsync(_user, _cancellationToken), Times.Once);
    _sessionRepository.Verify(x => x.SaveAsync(
      It.Is<Session>(s => s.CreatedBy.HasValue && s.UpdatedBy.HasValue && s.CreatedBy == s.UpdatedBy && s.CreatedBy.Value.Value == s.UserId.Value
        && !s.RealmId.HasValue && s.EntityId != Guid.Empty
        && s.UserId == _user.Id && !s.IsPersistent && s.IsActive && s.CustomAttributes.Count == 0),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should create a new persistent session.")]
  public async Task Given_Persistent_When_HandleAsync_Then_SessionCreated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    SessionDto session = new();
    _sessionQuerier.Setup(x => x.ReadAsync(It.IsAny<Session>(), _cancellationToken)).ReturnsAsync(session);

    User user = new(_user.UniqueName, password: null, actorId, UserId.NewId(realmId));

    Email email = new(_faker.Person.Email, isVerified: true);
    user.SetEmail(email);
    Assert.NotNull(user.Email);
    _userService.Setup(x => x.FindAsync(user.Email.Address, _cancellationToken)).ReturnsAsync(new FoundUsers(null, null, user, null));

    string secretString = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    Base64Password secret = new(secretString);
    _passwordService.Setup(x => x.GenerateBase64(RefreshToken.SecretLength, out secretString)).Returns(secret);

    CreateSessionPayload payload = new(user.Email.Address, isPersistent: true)
    {
      Id = Guid.NewGuid()
    };
    payload.CustomAttributes.Add(new CustomAttribute("IpAddress", _faker.Internet.Ip()));
    payload.CustomAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"));
    CreateSession command = new(payload);
    SessionDto result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Same(session, result);

    Assert.NotNull(result.RefreshToken);
    RefreshToken refreshToken = RefreshToken.Decode(result.RefreshToken, realmId);
    Assert.Equal(realmId, refreshToken.SessionId.RealmId);
    Assert.Equal(payload.Id.Value, refreshToken.SessionId.EntityId);
    Assert.Equal(secretString, refreshToken.Secret);

    Assert.Contains(user.Changes, change => change is UserSignedIn signedIn && signedIn.ActorId == actorId);

    _userService.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);
    _sessionRepository.Verify(x => x.SaveAsync(
      It.Is<Session>(s => s.CreatedBy.HasValue && s.UpdatedBy.HasValue && s.CreatedBy == s.UpdatedBy && s.CreatedBy == actorId
        && s.RealmId == realmId && s.EntityId == payload.Id.Value
        && s.UserId == user.Id && s.IsPersistent && s.IsActive && s.CustomAttributes.Count == 2),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already used.")]
  public async Task Given_IdAlreadyUsed_When_HandleAsync_Then_IdAlreadyUsedException()
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Session session = new(_user, secret: null, actorId: null, SessionId.NewId(realmId));
    _sessionRepository.Setup(x => x.LoadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    CreateSessionPayload payload = new(_user.UniqueName.Value)
    {
      Id = session.EntityId
    };
    CreateSession command = new(payload);
    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException<Session>>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(realmId.ToGuid(), exception.RealmId);
    Assert.Equal("Session", exception.EntityType);
    Assert.Equal(payload.Id.Value, exception.EntityId);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user was not found.")]
  public async Task Given_UserNotFound_When_HandleAsync_Then_UserNotFoundException()
  {
    CreateSessionPayload payload = new(_faker.Internet.UserName());

    FoundUsers foundUsers = new(null, null, null, null);
    _userService.Setup(x => x.FindAsync(payload.User, _cancellationToken)).ReturnsAsync(foundUsers);

    CreateSession command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Null(exception.RealmId);
    Assert.Equal(payload.User, exception.User);
    Assert.Equal("User", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    CreateSessionPayload payload = new();
    payload.CustomAttributes.Add(new CustomAttribute("123_invalid", _faker.Internet.Ip()));

    CreateSession command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "User");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }
}
