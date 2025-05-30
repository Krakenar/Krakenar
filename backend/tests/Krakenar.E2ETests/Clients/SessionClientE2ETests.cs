using Bogus;
using Krakenar.Client.Sessions;
using Krakenar.Client.Users;
using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Logitar;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class SessionClientE2ETests : E2ETests
{
  private const string PasswordString = "P@s$W0rD";

  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  public SessionClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Sessions should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    KrakenarSettings.Realm = Realm.UniqueSlug;
    using HttpClient sessionClient = new();
    SessionClient sessions = new(sessionClient, KrakenarSettings);
    using HttpClient userClient = new();
    UserClient users = new(userClient, KrakenarSettings);

    Guid id = Guid.Parse("ff9543de-0651-4813-9bda-3ff69bfe3049");
    CreateOrReplaceUserPayload createOrReplaceUser = new("admin")
    {
      Email = new EmailPayload(_faker.Person.Email, isVerified: true),
      Password = new ChangePasswordPayload(PasswordString),
      IsDisabled = false
    };
    CreateOrReplaceUserResult userResult = await users.CreateOrReplaceAsync(createOrReplaceUser, id, version: null, _cancellationToken);
    Assert.NotNull(userResult.User);
    User user = userResult.User;

    SignInSessionPayload signInSession = new(user.UniqueName, PasswordString, isPersistent: true);
    signInSession.CustomAttributes.Add(new CustomAttribute("IpAddress", _faker.Internet.Ip()));
    Session session = await sessions.SignInAsync(signInSession, _cancellationToken);
    Assert.Equal(user, session.User);
    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Equal(signInSession.CustomAttributes, session.CustomAttributes);

    RenewSessionPayload renewSession = new(session.RefreshToken);
    renewSession.CustomAttributes.Add(new CustomAttribute("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"));
    session = await sessions.RenewAsync(renewSession, _cancellationToken);
    Assert.True(session.IsPersistent);
    Assert.NotNull(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Equal(2, session.CustomAttributes.Count);
    Assert.Contains(signInSession.CustomAttributes.Single(), session.CustomAttributes);
    Assert.Contains(renewSession.CustomAttributes.Single(), session.CustomAttributes);

    createOrReplaceUser = new(_faker.Person.UserName);
    userResult = await users.CreateOrReplaceAsync(createOrReplaceUser, id: null, version: null, _cancellationToken);
    Assert.NotNull(userResult.User);
    User otherUser = userResult.User;

    CreateSessionPayload createSession = new(otherUser.Id.ToString());
    Session otherSession = await sessions.CreateAsync(createSession, _cancellationToken);
    Assert.Equal(otherUser, otherSession.User);
    Assert.False(otherSession.IsPersistent);
    Assert.Null(otherSession.RefreshToken);
    Assert.True(otherSession.IsActive);
    Assert.Equal(createSession.CustomAttributes, otherSession.CustomAttributes);

    SearchSessionsPayload searchSessions = new()
    {
      IsActive = true,
      Sort = [new SessionSortOption(SessionSort.UpdatedOn, isDescending: true)],
      Skip = -1,
      Limit = 1
    };
    SearchResults<Session> results = await sessions.SearchAsync(searchSessions, _cancellationToken);
    Assert.True(results.Total > 1);
    Assert.Equal(otherSession.Id, Assert.Single(results.Items).Id);

    Assert.NotNull(await users.SignOutAsync(otherUser.Id, _cancellationToken));

    otherSession = (await sessions.ReadAsync(otherSession.Id, _cancellationToken))!;
    Assert.NotNull(otherSession);
    Assert.False(otherSession.IsActive);
    Assert.NotNull(otherSession.SignedOutBy);
    Assert.NotNull(otherSession.SignedOutOn);

    Assert.NotNull(await users.DeleteAsync(otherUser.Id, _cancellationToken));

    Assert.Null(await sessions.ReadAsync(otherSession.Id, _cancellationToken));

    using HttpClient contextualizedClient = new();
    KrakenarSettings.User = user.UniqueName;
    SessionClient contextualizedSessions = new(contextualizedClient, KrakenarSettings);

    session = (await contextualizedSessions.SignOutAsync(session.Id, _cancellationToken))!;
    Assert.NotNull(session);
    Assert.Equal(user.Id, session.UpdatedBy.Id);
    Assert.False(session.IsActive);
    Assert.NotNull(session.SignedOutBy);
    Assert.Equal(user.Id, session.SignedOutBy.Id);
    Assert.NotNull(session.SignedOutOn);
    Assert.Equal(DateTime.UtcNow, session.SignedOutOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));
  }
}
