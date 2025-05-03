using Bogus;
using Krakenar.Contracts;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Passwords;
using Krakenar.Core.Realms;
using Krakenar.Core.Sessions.Events;
using Krakenar.Core.Settings;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class RenewSessionHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();

  private readonly RenewSessionHandler _handler;

  public RenewSessionHandlerTests()
  {
    _handler = new(_applicationContext.Object, _passwordManager.Object, _sessionQuerier.Object, _sessionRepository.Object);
  }

  [Theory(DisplayName = "It should renew the session.")]
  [InlineData(null)]
  [InlineData("f6de3bbc-61ba-41ba-94e4-9b8f4dec1d69")]
  public async Task Given_Session_When_HandleAsync_Then_Renewed(string? actorIdValue)
  {
    ActorId? actorId = actorIdValue is null ? null : new(Guid.Parse(actorIdValue));
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(new UniqueName(new UniqueNameSettings(), _faker.Person.UserName), userId: UserId.NewId(realmId));

    string secretString = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    Session session = new(user, new Base64Password(secretString));
    session.SetCustomAttribute(new Identifier("IpAddress"), _faker.Internet.Ip());
    session.Update();
    _sessionRepository.Setup(x => x.LoadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    string newSecretString = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    Base64Password newSecret = new(newSecretString);
    _passwordManager.Setup(x => x.GenerateBase64(RefreshToken.SecretLength, out newSecretString)).Returns(newSecret);

    SessionDto dto = new();
    _sessionQuerier.Setup(x => x.ReadAsync(session, _cancellationToken)).ReturnsAsync(dto);

    RenewSessionPayload payload = new(RefreshToken.Encode(session, secretString));
    payload.CustomAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"));
    RenewSession command = new(payload);
    SessionDto result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Same(dto, result);

    Assert.NotNull(result.RefreshToken);
    Assert.Equal(RefreshToken.Encode(session, newSecretString), result.RefreshToken);

    Assert.Equal(2, session.CustomAttributes.Count);
    Assert.Contains(session.CustomAttributes, c => c.Key.Value == "IpAddress");
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, session.CustomAttributes[new Identifier(customAttribute.Key)]);
    }

    Assert.Contains(session.Changes, change => change is SessionRenewed renewed
      && renewed.Secret == newSecret
      && renewed.ActorId == (actorId ?? new ActorId(user.Id.Value)));
    Assert.Contains(session.Changes, change => change is SessionUpdated updated
      && updated.CustomAttributes.Count == 1
      && updated.ActorId == (actorId ?? new ActorId(user.Id.Value)));

    _sessionRepository.Verify(x => x.SaveAsync(session, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw InvalidRefreshTokenException when the refresh token is not valid.")]
  public async Task Given_InvalidRefreshToken_When_HandleAsync_Then_InvalidRefreshTokenException()
  {
    RenewSessionPayload payload = new("invalid");
    RenewSession command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidRefreshTokenException>(async () => await _handler.HandleAsync(command, _cancellationToken));
    Assert.Equal(payload.RefreshToken, exception.RefreshToken);
    Assert.Equal("RefreshToken", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw SessionNotFoundException when the session was not found.")]
  public async Task Given_SessionNotFound_When_HandleAsync_Then_SessionNotFoundException()
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    SessionId sessionId = SessionId.NewId(realmId);
    string secret = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);

    RenewSessionPayload payload = new(RefreshToken.Encode(sessionId, secret));
    RenewSession command = new(payload);
    var exception = await Assert.ThrowsAsync<SessionNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));
    Assert.Equal(realmId.ToGuid(), exception.RealmId);
    Assert.Equal(sessionId.EntityId, exception.SessionId);
    Assert.Equal("RefreshToken", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    RenewSessionPayload payload = new();
    payload.CustomAttributes.Add(new CustomAttribute("123_invalid", _faker.Internet.Ip()));

    RenewSession command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "RefreshToken");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }
}
