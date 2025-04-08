using Bogus;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions.Events;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Moq;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SignOutSessionHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();

  private readonly SignOutSessionHandler _handler;

  public SignOutSessionHandlerTests()
  {
    _handler = new(_applicationContext.Object, _sessionQuerier.Object, _sessionRepository.Object);
  }

  [Fact(DisplayName = "It should return null when the session was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    SignOutSession command = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should sign-out the session.")]
  public async Task Given_Session_When_HandleAsync_Then_SignedOut()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), userId: UserId.NewId(realmId));
    Session session = new(user);
    _sessionRepository.Setup(x => x.LoadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    SessionDto dto = new();
    _sessionQuerier.Setup(x => x.ReadAsync(session, _cancellationToken)).ReturnsAsync(dto);

    SignOutSession command = new(session.EntityId);
    SessionDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.False(session.IsActive);
    Assert.Contains(session.Changes, change => change is SessionSignedOut signedOut && signedOut.ActorId == actorId);

    _sessionRepository.Verify(x => x.SaveAsync(session, _cancellationToken), Times.Once);
  }
}
