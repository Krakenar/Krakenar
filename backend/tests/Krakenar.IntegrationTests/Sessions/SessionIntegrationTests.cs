using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Core;
using Krakenar.Core.Passwords;
using Krakenar.Core.Sessions;
using Krakenar.Core.Users;
using Microsoft.Extensions.DependencyInjection;
using Session = Krakenar.Core.Sessions.Session;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Sessions;

[Trait(Traits.Category, Categories.Integration)]
public class SessionIntegrationTests : IntegrationTests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly IPasswordService _passwordService;
  private readonly ISessionRepository _sessionRepository;
  private readonly ISessionService _sessionService;
  private readonly IUserRepository _userRepository;

  private readonly User _user;

  public SessionIntegrationTests() : base()
  {
    _passwordService = ServiceProvider.GetRequiredService<IPasswordService>();
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _sessionService = ServiceProvider.GetRequiredService<ISessionService>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

    Password password = _passwordService.ValidateAndHash(PasswordString, Realm.PasswordSettings);
    _user = new(new UniqueName(Realm.UniqueNameSettings, Faker.Person.UserName), password, actorId: null, UserId.NewId(Realm.Id));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _userRepository.SaveAsync(_user);
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

  [Fact(DisplayName = "It should return null when the session cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _sessionService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Sessions_When_Search_Then_CorrectResults()
  {
    User other = new(new UniqueName(Realm.UniqueNameSettings, Faker.Internet.UserName()), password: null, ActorId, UserId.NewId(Realm.Id));
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
      Search = new TextSearch([new SearchTerm("test")]),
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
    Session persistent = new(_user, _passwordService.GenerateBase64(RefreshToken.SecretLength, out _));
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
}
