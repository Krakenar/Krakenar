using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Krakenar.Models.Account;
using Krakenar.Web.Constants;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Krakenar;

[Trait(Traits.Category, Categories.EndToEnd)]
public class SessionE2ETests
{
  private readonly Uri _baseUri;
  private readonly JsonSerializerOptions _serializerOptions = new();

  public SessionE2ETests()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    _serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();
    _baseUri = new Uri(configuration.GetValue<string>("BaseUrl") ?? string.Empty, UriKind.Absolute);
  }

  [Fact(DisplayName = "Session authentication should work correctly.")]
  public async Task Given_ValidCredentials_When_SignIn_Then_SessionAuthenticated()
  {
    using HttpClient client = new();
    client.BaseAddress = _baseUri;

    Uri requestUri = new("/api/sign/in", UriKind.Relative);
    using HttpRequestMessage signInRequest = new(HttpMethod.Post, requestUri);
    SignInAccountPayload payload = new("admin", "P@s$W0rD");
    signInRequest.Content = JsonContent.Create(payload, mediaType: null, _serializerOptions);

    using var signInResponse = await client.SendAsync(signInRequest);
    signInResponse.EnsureSuccessStatusCode();
    var currentUser = await signInResponse.Content.ReadFromJsonAsync<CurrentUser>();
    Assert.NotNull(currentUser);

    Assert.True(signInResponse.Headers.TryGetValues("Set-Cookie", out var values));
    var session = values.Single(v => v.StartsWith(".AspNetCore.Session="));

    requestUri = new($"/api/users/{currentUser.Id}", UriKind.Relative);
    using HttpRequestMessage readUserRequest = new(HttpMethod.Get, requestUri);
    readUserRequest.Headers.Add("Cookie", session);

    using var readUserResponse = await client.SendAsync(readUserRequest);
    readUserResponse.EnsureSuccessStatusCode();
    var user = await readUserResponse.Content.ReadFromJsonAsync<User>(_serializerOptions);
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
    using HttpClient client = new();
    client.BaseAddress = _baseUri;

    Uri requestUri = new("/api/sign/in", UriKind.Relative);
    using HttpRequestMessage signInRequest = new(HttpMethod.Post, requestUri);
    SignInAccountPayload payload = new("admin", "P@s$W0rD");
    signInRequest.Content = JsonContent.Create(payload, mediaType: null, _serializerOptions);

    using var signInResponse = await client.SendAsync(signInRequest);
    signInResponse.EnsureSuccessStatusCode();
    var currentUser = await signInResponse.Content.ReadFromJsonAsync<CurrentUser>();
    Assert.NotNull(currentUser);

    Assert.True(signInResponse.Headers.TryGetValues("Set-Cookie", out var values));
    var refreshToken = values.Single(v => v.StartsWith(Cookies.RefreshToken));

    requestUri = new($"/api/sessions/{currentUser.SessionId}", UriKind.Relative);
    using HttpRequestMessage readSessionRequest = new(HttpMethod.Get, requestUri);
    readSessionRequest.Headers.Add("Cookie", refreshToken);

    using var readSessionResponse = await client.SendAsync(readSessionRequest);
    readSessionResponse.EnsureSuccessStatusCode();
    var session = await readSessionResponse.Content.ReadFromJsonAsync<Session>(_serializerOptions);
    Assert.NotNull(session);
    Assert.Equal(currentUser.SessionId, session.Id);
    Assert.Equal(currentUser.Id, session.User.Id);
    Assert.True(session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.NotEmpty(session.CustomAttributes);

    Assert.True(readSessionResponse.Headers.TryGetValues("Set-Cookie", out values));
    Assert.Contains(values, value => value.StartsWith(".AspNetCore.Session="));
    Assert.NotEqual(refreshToken, values.Single(value => value.StartsWith(Cookies.RefreshToken)));
  }
}
