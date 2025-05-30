using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Krakenar.Models.Account;
using Krakenar.Web.Constants;

namespace Krakenar;

[Trait(Traits.Category, Categories.EndToEnd)]
public class SessionE2ETests : E2ETests
{
  public SessionE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Session authentication should work correctly.")]
  public async Task Given_ValidCredentials_When_SignIn_Then_SessionAuthenticated()
  {
    Uri requestUri = new("/api/sign/in", UriKind.Relative);
    using HttpRequestMessage signInRequest = new(HttpMethod.Post, requestUri);
    Assert.NotNull(KrakenarSettings.Basic);
    SignInAccountPayload payload = new(KrakenarSettings.Basic.Username.Trim(), KrakenarSettings.Basic.Password.Trim());
    signInRequest.Content = JsonContent.Create(payload, mediaType: null, SerializerOptions);

    using HttpResponseMessage signInResponse = await HttpClient.SendAsync(signInRequest);
    signInResponse.EnsureSuccessStatusCode();
    CurrentUser? currentUser = await signInResponse.Content.ReadFromJsonAsync<CurrentUser>();
    Assert.NotNull(currentUser);

    Assert.True(signInResponse.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? cookies));
    string session = cookies.Single(v => v.StartsWith(".AspNetCore.Session="));

    requestUri = new($"/api/users/{currentUser.Id}", UriKind.Relative);
    using HttpRequestMessage readUserRequest = new(HttpMethod.Get, requestUri);
    readUserRequest.Headers.Add("Cookie", session);

    using HttpResponseMessage readUserResponse = await HttpClient.SendAsync(readUserRequest);
    readUserResponse.EnsureSuccessStatusCode();
    User? user = await readUserResponse.Content.ReadFromJsonAsync<User>(SerializerOptions);
    Assert.NotNull(user);

    Assert.Equal(currentUser.Id, user.Id);
    Assert.Equal(currentUser.DisplayName, user.FullName ?? user.UniqueName);
    Assert.Equal(currentUser.EmailAddress, user.Email?.Address);
    Assert.Equal(currentUser.PictureUrl, user.Picture);

    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, user.AuthenticatedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "Session renewal middleware should work correctly.")]
  public async Task Given_RefreshToken_When_RenewSession_Then_SessionRenewed()
  {
    Uri requestUri = new("/api/sign/in", UriKind.Relative);
    using HttpRequestMessage signInRequest = new(HttpMethod.Post, requestUri);
    Assert.NotNull(KrakenarSettings.Basic);
    SignInAccountPayload payload = new(KrakenarSettings.Basic.Username.Trim(), KrakenarSettings.Basic.Password.Trim());
    signInRequest.Content = JsonContent.Create(payload, mediaType: null, SerializerOptions);

    using HttpResponseMessage signInResponse = await HttpClient.SendAsync(signInRequest);
    signInResponse.EnsureSuccessStatusCode();
    CurrentUser? currentUser = await signInResponse.Content.ReadFromJsonAsync<CurrentUser>();
    Assert.NotNull(currentUser);

    Assert.True(signInResponse.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? cookies));
    string refreshToken = cookies.Single(v => v.StartsWith(Cookies.RefreshToken));

    requestUri = new($"/api/sessions/{currentUser.SessionId}", UriKind.Relative);
    using HttpRequestMessage readSessionRequest = new(HttpMethod.Get, requestUri);
    readSessionRequest.Headers.Add("Cookie", refreshToken);

    using HttpResponseMessage readSessionResponse = await HttpClient.SendAsync(readSessionRequest);
    readSessionResponse.EnsureSuccessStatusCode();
    Session? session = await readSessionResponse.Content.ReadFromJsonAsync<Session>(SerializerOptions);
    Assert.NotNull(session);
    Assert.Equal(currentUser.SessionId, session.Id);
    Assert.Equal(currentUser.Id, session.User.Id);
    Assert.True(session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.NotEmpty(session.CustomAttributes);

    Assert.True(readSessionResponse.Headers.TryGetValues("Set-Cookie", out cookies));
    Assert.Contains(cookies, value => value.StartsWith(".AspNetCore.Session="));
    Assert.NotEqual(refreshToken, cookies.Single(value => value.StartsWith(Cookies.RefreshToken)));
  }
}
