using Krakenar.Contracts.Users;
using Krakenar.Models.Account;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Krakenar.Authentication;

[Trait(Traits.Category, Categories.EndToEnd)]
public class SessionAuthenticationTests
{
  private readonly Uri _baseUri;
  private readonly JsonSerializerOptions _serializerOptions = new();

  public SessionAuthenticationTests()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    _serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();
    _baseUri = new Uri(configuration.GetValue<string>("BaseUrl") ?? string.Empty, UriKind.Absolute);
  }

  [Fact(DisplayName = "Session authentication should work correctly.")]
  public async Task Given_ValidCredentials_When_SignIn_Then_SessionAuthentication()
  {
    using HttpClient client = new();
    client.BaseAddress = _baseUri;

    Uri requestUri = new("/api/sign/in", UriKind.Relative);
    using HttpRequestMessage signInRequest = new(HttpMethod.Post, requestUri);
    SignInAccountPayload payload = new("admin", "P@s$W0rD");
    signInRequest.Content = JsonContent.Create(payload, mediaType: null, _serializerOptions);

    using HttpResponseMessage signInResponse = await client.SendAsync(signInRequest);
    signInResponse.EnsureSuccessStatusCode();
    CurrentUser? currentUser = await signInResponse.Content.ReadFromJsonAsync<CurrentUser>();
    Assert.NotNull(currentUser);

    Assert.True(signInResponse.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? values));
    string session = values.Single(v => v.StartsWith(".AspNetCore.Session="));

    requestUri = new($"/api/users/{currentUser.Id}", UriKind.Relative);
    using HttpRequestMessage readUserRequest = new(HttpMethod.Get, requestUri);
    readUserRequest.Headers.Add("Cookie", session);

    using HttpResponseMessage readUserResponse = await client.SendAsync(readUserRequest);
    readUserResponse.EnsureSuccessStatusCode();
    User? user = await readUserResponse.Content.ReadFromJsonAsync<User>(_serializerOptions);
    Assert.NotNull(user);

    Assert.Equal(currentUser.Id, user.Id);
    Assert.Equal(currentUser.DisplayName, user.FullName ?? user.UniqueName);
    Assert.Equal(currentUser.EmailAddress, user.Email?.Address);
    Assert.Equal(currentUser.PictureUrl, user.Picture);

    Assert.NotNull(user.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, user.AuthenticatedOn.Value, TimeSpan.FromSeconds(10));
  }
}
