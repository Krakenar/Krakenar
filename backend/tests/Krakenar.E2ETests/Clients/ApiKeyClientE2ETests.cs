using Krakenar.Client.ApiKeys;
using Krakenar.Client.Roles;
using Krakenar.Contracts;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class ApiKeyClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public ApiKeyClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "API keys should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    KrakenarSettings.Realm = Realm.UniqueSlug;
    using HttpClient roleClient = new();
    RoleClient roles = new(roleClient, KrakenarSettings);
    using HttpClient apiKeyClient = new();
    ApiKeyClient apiKeys = new(apiKeyClient, KrakenarSettings);

    CreateOrReplaceRolePayload createOrReplaceRole = new("editor");
    Role? editor = await roles.ReadAsync(id: null, createOrReplaceRole.UniqueName, _cancellationToken);
    if (editor is null)
    {
      CreateOrReplaceRoleResult roleResult = await roles.CreateOrReplaceAsync(createOrReplaceRole, id: null, version: null, _cancellationToken);
      editor = roleResult.Role;
      Assert.NotNull(editor);
    }

    Guid id = Guid.Parse("95c3507a-91da-491e-bcfa-449cb2309bf1");
    ApiKey? apiKey = await apiKeys.ReadAsync(id, _cancellationToken);

    CreateOrReplaceApiKeyPayload createOrReplaceApiKey = new("Default API Key");
    CreateOrReplaceApiKeyResult apiKeyResult = await apiKeys.CreateOrReplaceAsync(createOrReplaceApiKey, id, version: null, _cancellationToken);
    Assert.Equal(apiKeyResult.Created, apiKey is null);
    apiKey = apiKeyResult.ApiKey;
    Assert.NotNull(apiKey);
    Assert.Equal(id, apiKey.Id);
    Assert.Equal(createOrReplaceApiKey.Name, apiKey.Name);

    UpdateApiKeyPayload updateApiKey = new()
    {
      Description = new Change<string>("  This is the default API key.  "),
      CustomAttributes = [new CustomAttribute("manage_api", bool.TrueString)],
      Roles = [new RoleChange($" {editor.UniqueName.ToUpperInvariant()} ")]
    };
    apiKey = await apiKeys.UpdateAsync(id, updateApiKey, _cancellationToken);
    Assert.NotNull(apiKey);
    Assert.Equal(createOrReplaceApiKey.Name, apiKey.Name);
    Assert.Equal(updateApiKey.Description.Value?.Trim(), apiKey.Description);
    Assert.Null(apiKey.ExpiresOn);
    Assert.Equal(updateApiKey.CustomAttributes, apiKey.CustomAttributes);
    Assert.Equal(editor, Assert.Single(apiKey.Roles));

    SearchApiKeysPayload searchApiKeys = new()
    {
      Ids = [apiKey.Id],
      Search = new TextSearch([new SearchTerm("def%")])
    };
    SearchResults<ApiKey> results = await apiKeys.SearchAsync(searchApiKeys, _cancellationToken);
    Assert.Equal(1, results.Total);
    apiKey = Assert.Single(results.Items);
    Assert.Equal(id, apiKey.Id);

    apiKeyResult = await apiKeys.CreateOrReplaceAsync(createOrReplaceApiKey, id: null, version: null, _cancellationToken);
    Assert.True(apiKeyResult.Created);
    apiKey = apiKeyResult.ApiKey;
    Assert.NotNull(apiKey);
    Assert.NotNull(apiKey.XApiKey);

    id = apiKey.Id;
    AuthenticateApiKeyPayload authenticatePayload = new(apiKey.XApiKey);
    apiKey = await apiKeys.AuthenticateAsync(authenticatePayload, _cancellationToken);
    Assert.Equal(id, apiKey.Id);

    apiKey = await apiKeys.DeleteAsync(id, _cancellationToken);
    Assert.NotNull(apiKey);
    Assert.Equal(id, apiKey.Id);
  }
}
