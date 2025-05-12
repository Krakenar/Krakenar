using Krakenar.Client.ApiKeys;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Constants;

namespace Krakenar;

[Trait(Traits.Category, Categories.EndToEnd)]
public class ApiKeyE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public ApiKeyE2ETests() : base()
  {
  }

  [Fact(DisplayName = "API Key authentication should work correctly.")]
  public async Task Given_ValidCredentials_When_SignIn_Then_ApiKeyAuthenticated()
  {
    using HttpClient httpClient = new();
    ApiKeyClient apiKeys = new(httpClient, KrakenarSettings);

    CreateOrReplaceApiKeyPayload payload = new("API Key Authentication Tests");
    CreateOrReplaceApiKeyResult apiKeyResult = await apiKeys.CreateOrReplaceAsync(payload, id: null, version: null, _cancellationToken);
    Assert.True(apiKeyResult.Created);
    ApiKey? apiKey = apiKeyResult.ApiKey;
    Assert.NotNull(apiKey);
    Assert.Null(apiKey.Realm);
    Assert.NotNull(apiKey.XApiKey);
    Guid apiKeyId = apiKey.Id;

    Uri requestUri = new($"/api/keys/{apiKeyId}", UriKind.Relative);
    using HttpRequestMessage request = new(HttpMethod.Get, requestUri);
    request.Headers.Add(Headers.ApiKey, apiKey.XApiKey);

    using HttpResponseMessage response = await HttpClient.SendAsync(request);
    response.EnsureSuccessStatusCode();
    apiKey = await response.Content.ReadFromJsonAsync<ApiKey>(SerializerOptions);
    Assert.NotNull(apiKey);
    Assert.Equal(apiKeyId, apiKey.Id);

    apiKey = await apiKeys.DeleteAsync(apiKey.Id, _cancellationToken);
    Assert.NotNull(apiKey);
    Assert.Equal(apiKeyId, apiKey.Id);
  }
}
