﻿using Bogus;
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
public class SignInSessionHandlerTests
{
  private const string Password = "P@s$W0rD";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IPasswordManager> _passwordManager = new();
  private readonly Mock<ISessionQuerier> _sessionQuerier = new();
  private readonly Mock<ISessionRepository> _sessionRepository = new();
  private readonly Mock<IUserManager> _userManager = new();

  private readonly SignInSessionHandler _handler;

  private readonly User _user;

  public SignInSessionHandlerTests()
  {
    _handler = new(_applicationContext.Object, _passwordManager.Object, _sessionQuerier.Object, _sessionRepository.Object, _userManager.Object);

    UniqueName uniqueName = new(new UniqueNameSettings(), _faker.Person.UserName);
    _user = new User(uniqueName, new Base64Password(Password));
    _userManager.Setup(x => x.FindAsync(_user.UniqueName.Value, _cancellationToken)).ReturnsAsync(new FoundUsers(null, _user, null, null));
  }

  [Fact(DisplayName = "It should create a new ephemereal session.")]
  public async Task Given_Ephemereal_When_HandleAsync_Then_SessionCreated()
  {
    SessionDto dto = new();
    _sessionQuerier.Setup(x => x.ReadAsync(It.IsAny<Session>(), _cancellationToken)).ReturnsAsync(dto);

    Session? session = null;
    _sessionRepository.Setup(x => x.SaveAsync(It.IsAny<Session>(), _cancellationToken)).Callback<Session, CancellationToken>((s, _) => session = s);

    SignInSessionPayload payload = new(_user.UniqueName.Value, Password);
    SignInSession command = new(payload);
    SessionDto result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Same(dto, result);
    Assert.Null(result.RefreshToken);

    Assert.Contains(_user.Changes, change => change is UserSignedIn signedIn && signedIn.ActorId?.Value == _user.Id.Value);
    _userManager.Verify(x => x.SaveAsync(_user, _cancellationToken), Times.Once);

    Assert.NotNull(session);
    Assert.Equal(_user.Id.Value, session.CreatedBy?.Value);
    Assert.Equal(_user.Id.Value, session.UpdatedBy?.Value);
    Assert.Equal(_user.RealmId, session.RealmId);
    Assert.NotEqual(Guid.Empty, session.EntityId);
    Assert.Equal(_user.Id, session.UserId);
    Assert.False(session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.Empty(session.CustomAttributes);
  }

  [Fact(DisplayName = "It should create a new persistent session.")]
  public async Task Given_Persistent_When_HandleAsync_Then_SessionCreated()
  {
    _applicationContext.SetupGet(x => x.RequireConfirmedAccount).Returns(true);

    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    SessionDto dto = new();
    _sessionQuerier.Setup(x => x.ReadAsync(It.IsAny<Session>(), _cancellationToken)).ReturnsAsync(dto);

    User user = new(_user.UniqueName, new Base64Password(Password), actorId, UserId.NewId(realmId));

    Email email = new(_faker.Person.Email, isVerified: true);
    user.SetEmail(email);
    Assert.NotNull(user.Email);
    _userManager.Setup(x => x.FindAsync(user.Email.Address, _cancellationToken)).ReturnsAsync(new FoundUsers(null, null, user, null));

    string secretString = RandomStringGenerator.GetBase64String(RefreshToken.SecretLength, out _);
    Base64Password secret = new(secretString);
    _passwordManager.Setup(x => x.GenerateBase64(RefreshToken.SecretLength, out secretString)).Returns(secret);

    Session? session = null;
    _sessionRepository.Setup(x => x.SaveAsync(It.IsAny<Session>(), _cancellationToken)).Callback<Session, CancellationToken>((s, _) => session = s);

    SignInSessionPayload payload = new(user.Email.Address, Password, isPersistent: true)
    {
      Id = Guid.NewGuid()
    };
    payload.CustomAttributes.Add(new CustomAttribute("IpAddress", _faker.Internet.Ip()));
    payload.CustomAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"));
    SignInSession command = new(payload);
    SessionDto result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Same(dto, result);

    Assert.NotNull(result.RefreshToken);
    RefreshToken refreshToken = RefreshToken.Decode(result.RefreshToken, realmId);
    Assert.Equal(realmId, refreshToken.SessionId.RealmId);
    Assert.Equal(payload.Id.Value, refreshToken.SessionId.EntityId);
    Assert.Equal(secretString, refreshToken.Secret);

    Assert.Contains(user.Changes, change => change is UserSignedIn signedIn && signedIn.ActorId == actorId);
    _userManager.Verify(x => x.SaveAsync(user, _cancellationToken), Times.Once);

    Assert.NotNull(session);
    Assert.Equal(actorId, session.CreatedBy);
    Assert.Equal(actorId, session.UpdatedBy);
    Assert.Equal(realmId, session.RealmId);
    Assert.Equal(payload.Id.Value, session.EntityId);
    Assert.Equal(user.Id, session.UserId);
    Assert.True(session.IsPersistent);
    Assert.True(session.IsActive);

    Assert.Equal(payload.CustomAttributes.Count, session.CustomAttributes.Count);
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Equal(customAttribute.Value, session.CustomAttributes[new Identifier(customAttribute.Key)]);
    }
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already used.")]
  public async Task Given_IdAlreadyUsed_When_HandleAsync_Then_IdAlreadyUsedException()
  {
    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(_user.UniqueName, password: null, actorId: null, UserId.NewId(realmId));
    Session session = new(user, secret: null, actorId: null, SessionId.NewId(realmId));
    _sessionRepository.Setup(x => x.LoadAsync(session.Id, _cancellationToken)).ReturnsAsync(session);

    SignInSessionPayload payload = new(_user.UniqueName.Value, Password)
    {
      Id = session.EntityId
    };
    SignInSession command = new(payload);
    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException<Session>>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(realmId.ToGuid(), exception.RealmId);
    Assert.Equal("Session", exception.EntityType);
    Assert.Equal(payload.Id.Value, exception.EntityId);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UserIsNotConfirmedException when the user is not confirmed.")]
  public async Task Given_UserNotConfirmed_When_HandleAsync_Then_UserIsNotConfirmedException()
  {
    _applicationContext.SetupGet(x => x.RequireConfirmedAccount).Returns(true);

    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    User user = new(_user.UniqueName, new Base64Password(Password), actorId, UserId.NewId(realmId));
    _userManager.Setup(x => x.FindAsync(user.UniqueName.Value, _cancellationToken)).ReturnsAsync(new FoundUsers(null, user, null, null));

    SignInSession command = new(new SignInSessionPayload(user.UniqueName.Value, "invalid"));
    var exception = await Assert.ThrowsAsync<UserIsNotConfirmedException>(async () => await _handler.HandleAsync(command, _cancellationToken));
    Assert.Equal(realmId.ToGuid(), exception.RealmId);
    Assert.Equal(user.EntityId, exception.UserId);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user was not found.")]
  public async Task Given_UserNotFound_When_HandleAsync_Then_UserNotFoundException()
  {
    SignInSessionPayload payload = new(_faker.Internet.UserName(), Password);

    FoundUsers foundUsers = new(null, null, null, null);
    _userManager.Setup(x => x.FindAsync(payload.UniqueName, _cancellationToken)).ReturnsAsync(foundUsers);

    SignInSession command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Null(exception.RealmId);
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    SignInSessionPayload payload = new();
    payload.CustomAttributes.Add(new CustomAttribute("123_invalid", _faker.Internet.Ip()));

    SignInSession command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "UniqueName");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "IdentifierValidator" && e.PropertyName == "CustomAttributes[0].Key");
  }
}
