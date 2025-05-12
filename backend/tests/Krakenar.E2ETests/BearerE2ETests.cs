using Krakenar.Contracts.Constants;
using Krakenar.Contracts.Users;
using Krakenar.Models.Account;
using Krakenar.Web.Models.Authentication;

namespace Krakenar;

[Trait(Traits.Category, Categories.EndToEnd)]
public class BearerE2ETests : E2ETests
{
  public BearerE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Bearer authentication should work correctly.")]
  public async Task Given_ValidCredentials_When_SignIn_Then_BearerAuthenticated()
  {
    Uri requestUri = new("/api/auth/token", UriKind.Relative);
    using HttpRequestMessage getTokenRequest = new(HttpMethod.Post, requestUri);
    Assert.NotNull(KrakenarSettings.Basic);
    SignInAccountPayload credentials = new(KrakenarSettings.Basic.Username.Trim(), KrakenarSettings.Basic.Password.Trim());
    GetTokenPayload payload = new(credentials);
    getTokenRequest.Content = JsonContent.Create(payload, mediaType: null, SerializerOptions);

    using HttpResponseMessage getTokenResponse = await HttpClient.SendAsync(getTokenRequest);
    getTokenResponse.EnsureSuccessStatusCode();
    TokenResponse? tokenResponse = await getTokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
    Assert.NotNull(tokenResponse);
    Assert.Equal(Schemes.Bearer, tokenResponse.TokenType);
    Assert.NotEmpty(tokenResponse.AccessToken);
    Assert.True(tokenResponse.ExpiresIn > 0);
    Assert.NotNull(tokenResponse.RefreshToken);
    Assert.Null(tokenResponse.Scope);

    requestUri = new($"/api/users/name:{credentials.Username}", UriKind.Relative);
    using HttpRequestMessage readUserRequest = new(HttpMethod.Get, requestUri);
    readUserRequest.Headers.Add(Headers.Authorization, string.Join(' ', tokenResponse.TokenType, tokenResponse.AccessToken));

    using HttpResponseMessage readUserResponse = await HttpClient.SendAsync(readUserRequest);
    readUserResponse.EnsureSuccessStatusCode();
    User? user = await readUserResponse.Content.ReadFromJsonAsync<User>(SerializerOptions);
    Assert.NotNull(user);
    Assert.Equal(credentials.Username, user.UniqueName);
    Assert.True(user.HasPassword);

    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, user.AuthenticatedOn.Value, TimeSpan.FromSeconds(1));

    requestUri = new("/api/auth/token", UriKind.Relative);
    payload = new GetTokenPayload(tokenResponse.RefreshToken);
    using HttpRequestMessage refreshTokenRequest = new(HttpMethod.Post, requestUri);
    refreshTokenRequest.Content = JsonContent.Create(payload, mediaType: null, SerializerOptions);

    using HttpResponseMessage refreshTokenResponse = await HttpClient.SendAsync(refreshTokenRequest);
    refreshTokenResponse.EnsureSuccessStatusCode();
    tokenResponse = await refreshTokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
    Assert.NotNull(tokenResponse);
    Assert.Equal(Schemes.Bearer, tokenResponse.TokenType);
    Assert.NotEmpty(tokenResponse.AccessToken);
    Assert.True(tokenResponse.ExpiresIn > 0);
    Assert.NotNull(tokenResponse.RefreshToken);
    Assert.Null(tokenResponse.Scope);
  }
}
