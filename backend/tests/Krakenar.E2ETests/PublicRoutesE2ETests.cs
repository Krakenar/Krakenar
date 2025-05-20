using Krakenar.Models.Index;

namespace Krakenar;

[Trait(Traits.Category, Categories.EndToEnd)]
public class PublicRoutesE2ETests : E2ETests
{
  public PublicRoutesE2ETests() : base()
  {
  }

  [Fact(DisplayName = "404 Not Found should be redirected to frontend app.")]
  public async Task Given_API_When_NotFound_Then_Redirected()
  {
    using HttpClientHandler handler = new()
    {
      AllowAutoRedirect = false
    };
    using HttpClient httpClient = new(handler);
    if (!string.IsNullOrWhiteSpace(KrakenarSettings.BaseUrl))
    {
      httpClient.BaseAddress = new Uri(KrakenarSettings.BaseUrl.Trim(), UriKind.Absolute);
    }

    Uri requestUri = new("/404", UriKind.Relative);
    using HttpRequestMessage request = new(HttpMethod.Get, requestUri);

    using HttpResponseMessage response = await httpClient.SendAsync(request);
    Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

    Assert.True(response.Headers.TryGetValues("Location", out IEnumerable<string>? locations));
    Assert.Equal("/app/404", locations.Single());
  }

  [Fact(DisplayName = "API index should return the current version.")]
  public async Task Given_API_When_Index_Then_ApiVersion()
  {
    Uri requestUri = new("/api", UriKind.Relative);
    using HttpRequestMessage request = new(HttpMethod.Get, requestUri);

    using HttpResponseMessage response = await HttpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    ApiVersion? apiVersion = await response.Content.ReadFromJsonAsync<ApiVersion>(SerializerOptions);
    Assert.NotNull(apiVersion);
    Assert.NotEmpty(apiVersion.Title);
    Assert.NotEmpty(apiVersion.Version);
  }

  [Fact(DisplayName = "Health checks should return Healthy.")]
  public async Task Given_API_When_Health_Then_Healthy()
  {
    Uri requestUri = new("/health", UriKind.Relative);
    using HttpRequestMessage request = new(HttpMethod.Get, requestUri);

    using HttpResponseMessage response = await HttpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    string content = await response.Content.ReadAsStringAsync();
    Assert.Equal("Healthy", content);
  }

  [Fact(DisplayName = "Static files should be downloaded.")]
  public async Task Given_API_When_StaticFiles_Then_Downloaded()
  {
    Uri requestUri = new("/assets/index.js", UriKind.Relative);
    using HttpRequestMessage request = new(HttpMethod.Get, requestUri);

    using HttpResponseMessage response = await HttpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    string content = await response.Content.ReadAsStringAsync();
    Assert.NotEmpty(content);
  }

  [Fact(DisplayName = "Swagger should return Swagger UI.")]
  public async Task Given_API_When_Swagger_Then_Swagger()
  {
    Uri requestUri = new("/swagger/index.html", UriKind.Relative);
    using HttpRequestMessage request = new(HttpMethod.Get, requestUri);

    using HttpResponseMessage response = await HttpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();

    string content = await response.Content.ReadAsStringAsync();
    Assert.NotEmpty(content);
  }
}
