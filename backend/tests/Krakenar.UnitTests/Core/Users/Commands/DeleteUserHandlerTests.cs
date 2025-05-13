using Bogus;
using Krakenar.Core.Passwords;
using Krakenar.Core.Passwords.Events;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions;
using Krakenar.Core.Sessions.Events;
using Krakenar.Core.Settings;
using Krakenar.Core.Users.Events;
using Logitar.EventSourcing;
using Moq;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteUserHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IOneTimePasswordQuerier> _oneTimePasswordQuerier = new();
  private readonly Mock<IOneTimePasswordRepository> _oneTimePasswordRepository = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly DeleteUserHandler _handler;

  public DeleteUserHandlerTests()
  {
    _handler = new(_applicationContext.Object, _oneTimePasswordQuerier.Object, _oneTimePasswordRepository.Object, _sessionQuerier.Object, _sessionRepository.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "It should delete the user and its One-Time Passwords (OTP).")]
  public async Task Given_UserOTPs_When_HandleAsync_Then_Delete()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), password: null, actorId, UserId.NewId(realmId));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    Base64Password password = new(_faker.Random.String(length: 6, minChar: '0', maxChar: '9'));
    OneTimePassword oneTimePassword1 = new(password, expiresOn: null, maximumAttempts: null, user, actorId, OneTimePasswordId.NewId(realmId));
    OneTimePassword oneTimePassword2 = new(password, expiresOn: null, maximumAttempts: null, user, actorId, OneTimePasswordId.NewId(realmId));
    _oneTimePasswordQuerier.Setup(x => x.FindIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([oneTimePassword1.Id, oneTimePassword2.Id]);
    _oneTimePasswordRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<OneTimePasswordId>>(y => y.SequenceEqual(new OneTimePasswordId[] { oneTimePassword1.Id, oneTimePassword2.Id })),
      _cancellationToken)).ReturnsAsync([oneTimePassword1, oneTimePassword2]);

    _sessionQuerier.Setup(x => x.FindIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([]);

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    DeleteUser command = new(user.EntityId);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(user.IsDeleted);
    Assert.Contains(user.Changes, change => change is UserDeleted deleted && deleted.ActorId == actorId);

    Assert.True(oneTimePassword1.IsDeleted);
    Assert.Contains(oneTimePassword1.Changes, change => change is OneTimePasswordDeleted deleted && deleted.ActorId == actorId);
    Assert.True(oneTimePassword2.IsDeleted);
    Assert.Contains(oneTimePassword2.Changes, change => change is OneTimePasswordDeleted deleted && deleted.ActorId == actorId);

    _oneTimePasswordRepository.Verify(x => x.SaveAsync(
      It.Is<IEnumerable<OneTimePassword>>(y => y.SequenceEqual(new OneTimePassword[] { oneTimePassword1, oneTimePassword2 })),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should delete the user and its sessions.")]
  public async Task Given_UserSessions_When_HandleAsync_Then_Delete()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), password: null, actorId, UserId.NewId(realmId));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    _oneTimePasswordQuerier.Setup(x => x.FindIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([]);

    Session session1 = new(user);
    Session session2 = new(user);
    _sessionQuerier.Setup(x => x.FindIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([session1.Id, session2.Id]);
    _sessionRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<SessionId>>(y => y.SequenceEqual(new SessionId[] { session1.Id, session2.Id })),
      _cancellationToken)).ReturnsAsync([session1, session2]);

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    DeleteUser command = new(user.EntityId);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(user.IsDeleted);
    Assert.Contains(user.Changes, change => change is UserDeleted deleted && deleted.ActorId == actorId);

    Assert.True(session1.IsDeleted);
    Assert.Contains(session1.Changes, change => change is SessionDeleted deleted && deleted.ActorId == actorId);
    Assert.True(session2.IsDeleted);
    Assert.Contains(session2.Changes, change => change is SessionDeleted deleted && deleted.ActorId == actorId);

    _sessionRepository.Verify(x => x.SaveAsync(
      It.Is<IEnumerable<Session>>(y => y.SequenceEqual(new Session[] { session1, session2 })),
      _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should not delete any One-Time Password (OTP) when there are none.")]
  public async Task Given_NoOTP_When_HandleAsync_Then_NoneDeleted()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    _oneTimePasswordQuerier.Setup(x => x.FindIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([]);
    _sessionQuerier.Setup(x => x.FindIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([]);

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    DeleteUser command = new(user.EntityId);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    _oneTimePasswordRepository.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<OneTimePassword>>(), _cancellationToken), Times.Never);
    _sessionRepository.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Session>>(), _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "It should not delete any session when there are none.")]
  public async Task Given_NoSession_When_HandleAsync_Then_NoneDeleted()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    _oneTimePasswordQuerier.Setup(x => x.FindIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([]);
    _sessionQuerier.Setup(x => x.FindIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([]);

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    DeleteUser command = new(user.EntityId);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    _sessionRepository.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Session>>(), _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "It should return null when the user was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteUser command = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }
}
