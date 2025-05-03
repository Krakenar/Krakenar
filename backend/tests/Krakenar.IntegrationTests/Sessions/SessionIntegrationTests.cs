using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Core;
using Krakenar.Core.Passwords;
using Krakenar.Core.Sessions;
using Krakenar.Core.Users;
using Microsoft.Extensions.DependencyInjection;
using CustomIdentifier = Krakenar.Core.CustomIdentifier;
using Session = Krakenar.Core.Sessions.Session;
using SessionDto = Krakenar.Contracts.Sessions.Session;
using TimeZone = Krakenar.Core.Localization.TimeZone;

namespace Krakenar.Sessions;

[Trait(Traits.Category, Categories.Integration)]
public class SessionIntegrationTests : IntegrationTests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly IPasswordManager _passwordManager;
  private readonly ISessionRepository _sessionRepository;
  private readonly ISessionService _sessionService;
  private readonly IUserRepository _userRepository;

  private readonly User _user;

  public SessionIntegrationTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _sessionService = ServiceProvider.GetRequiredService<ISessionService>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

    Password password = _passwordManager.ValidateAndHash(PasswordString, Realm.PasswordSettings);
    _user = new(new UniqueName(Realm.UniqueNameSettings, Faker.Person.UserName), password, actorId: null, UserId.NewId(Realm.Id))
    {
      TimeZone = new TimeZone("America/Montreal")
    };
    _user.Update();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _userRepository.SaveAsync(_user);
  }

  [Theory(DisplayName = "It should create a new session.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_User_When_Create_Then_Created(bool isPersistent)
  {
    Identifier key = new("Google");
    CustomIdentifier value = new("1234567890");
    _user.SetCustomIdentifier(key, value, ActorId);
    await _userRepository.SaveAsync(_user);

    CreateSessionPayload payload = new()
    {
      Id = Guid.NewGuid(),
      User = isPersistent ? string.Join(':', key, value) : _user.EntityId.ToString(),
      IsPersistent = isPersistent
    };
    payload.CustomAttributes.Add(new CustomAttribute("IpAddress", Faker.Internet.Ip()));
    payload.CustomAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"));

    SessionDto session = await _sessionService.CreateAsync(payload);

    Assert.Equal(payload.Id, session.Id);
    Assert.Equal(2, session.Version);
    Assert.Equal(Actor, session.CreatedBy);
    Assert.Equal(DateTime.UtcNow, session.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, session.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, session.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_user.EntityId, session.User.Id);
    Assert.Equal(isPersistent, session.IsPersistent);
    if (isPersistent)
    {
      Assert.NotNull(session.RefreshToken);
    }
    else
    {
      Assert.Null(session.RefreshToken);
    }
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);
  }

  [Fact(DisplayName = "It should read the session by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    Session session = new(_user);
    await _sessionRepository.SaveAsync(session);

    SessionDto? dto = await _sessionService.ReadAsync(session.EntityId);
    Assert.NotNull(dto);
    Assert.Equal(session.EntityId, dto.Id);
  }

  [Fact(DisplayName = "It should renew a session.")]
  public async Task Given_Session_When_Renew_Then_Renewed()
  {
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    Session session = new(_user, secret);
    session.SetCustomAttribute(new Identifier("IpAddress"), Faker.Internet.Ip());
    session.Update(ActorId);
    await _sessionRepository.SaveAsync(session);

    RenewSessionPayload payload = new()
    {
      RefreshToken = RefreshToken.Encode(session, secretString)
    };
    payload.CustomAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"));

    SessionDto renewed = await _sessionService.RenewAsync(payload);

    Assert.Equal(session.EntityId, renewed.Id);
    Assert.Equal(session.Version + 2, renewed.Version);
    Assert.Equal(Actor, renewed.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, renewed.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_user.EntityId, renewed.User.Id);
    Assert.True(renewed.IsPersistent);
    Assert.NotNull(renewed.RefreshToken);
    Assert.True(renewed.IsActive);
    Assert.Null(renewed.SignedOutBy);
    Assert.Null(renewed.SignedOutOn);

    Assert.Equal(2, renewed.CustomAttributes.Count);
    foreach (KeyValuePair<Identifier, string> customAttribute in session.CustomAttributes)
    {
      Assert.Contains(renewed.CustomAttributes, c => c.Key == customAttribute.Key.Value && c.Value == customAttribute.Value);
    }
    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      Assert.Contains(customAttribute, renewed.CustomAttributes);
    }
  }

  [Fact(DisplayName = "It should return null when the session cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _sessionService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Sessions_When_Search_Then_CorrectResults()
  {
    User other = new(new UniqueName(Realm.UniqueNameSettings, Faker.Internet.UserName()), password: null, ActorId, UserId.NewId(Realm.Id))
    {
      Gender = new Gender("other")
    };
    other.Update(ActorId);
    await _userRepository.SaveAsync(other);

    Session session1 = new(_user);
    Session session2 = new(_user);
    Session session3 = new(_user);
    Session session4 = new(other);
    await _sessionRepository.SaveAsync([session1, session2, session3]);

    SearchSessionsPayload payload = new()
    {
      UserId = _user.EntityId,
      Ids = [session1.EntityId, session2.EntityId, session4.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("%Montreal"), new SearchTerm("other")], SearchOperator.Or),
      Sort = [new SessionSortOption(SessionSort.UpdatedOn, isDescending: true)],
      Skip = 1,
      Limit = 1
    };

    SearchResults<SessionDto> sessions = await _sessionService.SearchAsync(payload);
    Assert.Equal(2, sessions.Total);

    SessionDto user = Assert.Single(sessions.Items);
    Assert.Equal(session1.EntityId, user.Id);
  }

  [Theory(DisplayName = "It should return the correct search results (IsActive).")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_IsActive_When_Search_Then_CorrectResults(bool isActive)
  {
    Session active = new(_user);
    Session inactive = new(_user);
    inactive.SignOut();
    await _sessionRepository.SaveAsync([active, inactive]);

    SearchSessionsPayload payload = new()
    {
      IsActive = isActive
    };

    SearchResults<SessionDto> sessions = await _sessionService.SearchAsync(payload);
    Assert.Equal(1, sessions.Total);

    SessionDto session = Assert.Single(sessions.Items);
    Assert.Equal(isActive ? active.EntityId : inactive.EntityId, session.Id);
  }

  [Theory(DisplayName = "It should return the correct search results (IsPersistent).")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_IsPersistent_When_Search_Then_CorrectResults(bool isPersistent)
  {
    Session ephemereal = new(_user);
    Session persistent = new(_user, _passwordManager.GenerateBase64(RefreshToken.SecretLength, out _));
    await _sessionRepository.SaveAsync([ephemereal, persistent]);

    SearchSessionsPayload payload = new()
    {
      IsPersistent = isPersistent
    };

    SearchResults<SessionDto> sessions = await _sessionService.SearchAsync(payload);
    Assert.Equal(1, sessions.Total);

    SessionDto session = Assert.Single(sessions.Items);
    Assert.Equal(isPersistent ? persistent.EntityId : ephemereal.EntityId, session.Id);
  }

  [Theory(DisplayName = "It should sign-in a user, creating a session.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_User_When_SignIn_Then_SignedIn(bool isPersistent)
  {
    Email email = new(Faker.Person.Email, isVerified: true);
    _user.SetEmail(email, ActorId);
    await _userRepository.SaveAsync(_user);

    SignInSessionPayload payload = new()
    {
      Id = Guid.NewGuid(),
      UniqueName = isPersistent ? email.Address : _user.UniqueName.Value,
      Password = PasswordString,
      IsPersistent = isPersistent
    };
    payload.CustomAttributes.Add(new CustomAttribute("IpAddress", Faker.Internet.Ip()));
    payload.CustomAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"));

    SessionDto session = await _sessionService.SignInAsync(payload);

    Assert.Equal(payload.Id, session.Id);
    Assert.Equal(2, session.Version);
    Assert.Equal(Actor, session.CreatedBy);
    Assert.Equal(DateTime.UtcNow, session.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, session.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, session.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_user.EntityId, session.User.Id);
    Assert.Equal(isPersistent, session.IsPersistent);
    if (isPersistent)
    {
      Assert.NotNull(session.RefreshToken);
    }
    else
    {
      Assert.Null(session.RefreshToken);
    }
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(payload.CustomAttributes, session.CustomAttributes);
  }

  [Fact(DisplayName = "It should sign-out a session.")]
  public async Task Given_Session_When_SignOut_Then_SignedOut()
  {
    Session session = new(_user);
    await _sessionRepository.SaveAsync(session);

    SessionDto? signedOut = await _sessionService.SignOutAsync(session.EntityId);

    Assert.NotNull(signedOut);
    Assert.Equal(session.EntityId, signedOut.Id);
    Assert.Equal(2, signedOut.Version);
    Assert.Equal(Actor, signedOut.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, signedOut.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_user.EntityId, signedOut.User.Id);
    Assert.False(signedOut.IsPersistent);
    Assert.False(signedOut.IsActive);
    Assert.Equal(Actor, signedOut.SignedOutBy);
    Assert.NotNull(signedOut.SignedOutOn);
    Assert.Equal(DateTime.UtcNow, signedOut.SignedOutOn.Value, TimeSpan.FromSeconds(10));
    Assert.Empty(signedOut.CustomAttributes);
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already used.")]
  public async Task Given_IdAlreadyUsed_When_Create_Then_IdAlreadyUsedException()
  {
    Session session = new(_user);
    await _sessionRepository.SaveAsync(session);

    CreateSessionPayload payload = new()
    {
      Id = session.EntityId,
      User = _user.EntityId.ToString()
    };

    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException<Session>>(async () => await _sessionService.CreateAsync(payload));
    Assert.Equal(RealmDto?.Id, exception.RealmId);
    Assert.Equal("Session", exception.EntityType);
    Assert.Equal(session.EntityId, exception.EntityId);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw SessionNotFoundException when the session was not found.")]
  public async Task Given_SessionNotFound_When_Renew_Then_SessionNotFoundException()
  {
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out string secretString);
    Session session = new(_user, secret);

    RenewSessionPayload payload = new()
    {
      RefreshToken = RefreshToken.Encode(session, secretString)
    };

    var exception = await Assert.ThrowsAsync<SessionNotFoundException>(async () => await _sessionService.RenewAsync(payload));
    Assert.Equal(RealmDto?.Id, exception.RealmId);
    Assert.Equal(session.EntityId, exception.SessionId);
    Assert.Equal("RefreshToken", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user was not found.")]
  public async Task Given_UserNotFound_When_SignIn_Then_UserNotFoundException()
  {
    SignInSessionPayload payload = new()
    {
      UniqueName = "not_found",
      Password = PasswordString
    };

    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await _sessionService.SignInAsync(payload));
    Assert.Equal(RealmDto?.Id, exception.RealmId);
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal("UniqueName", exception.PropertyName);
  }
}
