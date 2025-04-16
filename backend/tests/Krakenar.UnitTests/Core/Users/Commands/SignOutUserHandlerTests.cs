using Bogus;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions;
using Krakenar.Core.Sessions.Events;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using Moq;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignOutUserHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();
  private readonly Mock<IUserRepository> _userRepository = new();

  private readonly SignOutUserHandler _handler;

  public SignOutUserHandlerTests()
  {
    _handler = new(_applicationContext.Object, _sessionQuerier.Object, _sessionRepository.Object, _userQuerier.Object, _userRepository.Object);
  }

  [Fact(DisplayName = "It should not sign-out any session when none are active.")]
  public async Task Given_NoActive_When_HandleAsync_Then_NoneSignedOut()
  {
    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    _sessionQuerier.Setup(x => x.FindActiveIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([]);

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    SignOutUser command = new(user.EntityId);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    _sessionRepository.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Session>>(), _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "It should return null when the user was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    SignOutUser command = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should sign-out active user sessions.")]
  public async Task Given_UserSessions_When_HandleAsync_Then_SignOut()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), password: null, actorId, UserId.NewId(realmId));
    _userRepository.Setup(x => x.LoadAsync(user.Id, _cancellationToken)).ReturnsAsync(user);

    Session session1 = new(user);
    Session session2 = new(user);
    _sessionQuerier.Setup(x => x.FindActiveIdsAsync(user.Id, _cancellationToken)).ReturnsAsync([session1.Id, session2.Id]);
    _sessionRepository.Setup(x => x.LoadAsync(
      It.Is<IEnumerable<SessionId>>(y => y.SequenceEqual(new SessionId[] { session1.Id, session2.Id })),
      _cancellationToken)).ReturnsAsync([session1, session2]);

    UserDto dto = new();
    _userQuerier.Setup(x => x.ReadAsync(user, _cancellationToken)).ReturnsAsync(dto);

    SignOutUser command = new(user.EntityId);
    UserDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.False(session1.IsActive);
    Assert.Contains(session1.Changes, change => change is SessionSignedOut signedOut && signedOut.ActorId == actorId);
    Assert.False(session2.IsActive);
    Assert.Contains(session2.Changes, change => change is SessionSignedOut signedOut && signedOut.ActorId == actorId);

    _sessionRepository.Verify(x => x.SaveAsync(
      It.Is<IEnumerable<Session>>(y => y.SequenceEqual(new Session[] { session1, session2 })),
      _cancellationToken), Times.Once);
  }
}
