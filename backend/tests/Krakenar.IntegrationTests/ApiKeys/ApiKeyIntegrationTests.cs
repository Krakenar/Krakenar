using Krakenar.Contracts;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.ApiKeys;
using Krakenar.Core.Passwords;
using Krakenar.Core.Roles;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ApiKey = Krakenar.Core.ApiKeys.ApiKey;
using ApiKeyDto = Krakenar.Contracts.ApiKeys.ApiKey;
using Role = Krakenar.Core.Roles.Role;

namespace Krakenar.ApiKeys;

[Trait(Traits.Category, Categories.Integration)]
public class ApiKeyIntegrationTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IApiKeyService _apiKeyService;
  private readonly IRoleRepository _roleRepository;

  private Password _secret = null!;
  private string _secretString = null!;
  private ApiKey _apiKey = null!;

  public ApiKeyIntegrationTests() : base()
  {
    _apiKeyRepository = ServiceProvider.GetRequiredService<IApiKeyRepository>();
    _apiKeyService = ServiceProvider.GetRequiredService<IApiKeyService>();
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _secret = ServiceProvider.GetRequiredService<IPasswordManager>().GenerateBase64(XApiKey.SecretLength, out _secretString);
    _apiKey = new(_secret, new DisplayName("Test"), ActorId, ApiKeyId.NewId(Realm.Id));
    await _apiKeyRepository.SaveAsync(_apiKey);
  }

  [Fact(DisplayName = "It should authenticate an existing API key.")]
  public async Task Given_ApiKey_When_Authenticate_Then_Authenticated()
  {
    string xApiKey = XApiKey.Encode(_apiKey, _secretString);
    AuthenticateApiKeyPayload payload = new(xApiKey);

    ApiKeyDto apiKey = await _apiKeyService.AuthenticateAsync(payload);

    Assert.Equal(_apiKey.EntityId, apiKey.Id);
    Assert.Equal(_apiKey.Version + 1, apiKey.Version);
    Assert.Equal(Actor, apiKey.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, apiKey.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.NotNull(apiKey.AuthenticatedOn);
    Assert.Equal(DateTime.UtcNow, apiKey.AuthenticatedOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should create a new API key.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created()
  {
    Role admin = new(new UniqueName(Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync(admin);

    CreateOrReplaceApiKeyPayload payload = new()
    {
      Name = "Clé de test",
      Description = "Ceci est une clé de test.",
      ExpiresOn = DateTime.Now.AddYears(1)
    };
    payload.CustomAttributes.Add(new CustomAttribute("edit_documents", bool.TrueString));
    payload.CustomAttributes.Add(new CustomAttribute("publish_documents", bool.FalseString));
    payload.Roles.Add("  AdmIn  ");

    Guid id = Guid.NewGuid();
    CreateOrReplaceApiKeyResult result = await _apiKeyService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    ApiKeyDto? apiKey = result.ApiKey;
    Assert.NotNull(apiKey);
    Assert.Equal(id, apiKey.Id);
    Assert.Equal(3, apiKey.Version);
    Assert.Equal(Actor, apiKey.CreatedBy);
    Assert.Equal(DateTime.UtcNow, apiKey.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, apiKey.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, apiKey.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, apiKey.Realm);
    Assert.Equal(payload.Name, apiKey.Name);
    Assert.Equal(payload.Description.Trim(), apiKey.Description);
    Assert.NotNull(apiKey.ExpiresOn);
    Assert.Equal(payload.ExpiresOn.Value.AsUniversalTime(), apiKey.ExpiresOn.Value.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Null(apiKey.AuthenticatedOn);
    Assert.Equal(payload.CustomAttributes, apiKey.CustomAttributes);

    Assert.NotNull(apiKey.XApiKey);
    XApiKey xApiKey = XApiKey.Decode(apiKey.XApiKey, Realm.Id);
    Assert.Equal(Realm.Id, xApiKey.ApiKeyId.RealmId);
    Assert.Equal(id, xApiKey.ApiKeyId.EntityId);
    Assert.Equal(XApiKey.SecretLength, Convert.FromBase64String(xApiKey.Secret).Length);

    Assert.Single(apiKey.Roles);
    Assert.Contains(apiKey.Roles, r => r.Id == admin.EntityId);
  }

  [Fact(DisplayName = "It should delete the API key.")]
  public async Task Given_ApiKey_When_Delete_Then_Deleted()
  {
    ApiKeyDto? apiKey = await _apiKeyService.DeleteAsync(_apiKey.EntityId);
    Assert.NotNull(apiKey);
    Assert.Equal(_apiKey.EntityId, apiKey.Id);

    Assert.Empty(await KrakenarContext.ApiKeys.AsNoTracking().Where(x => x.StreamId == _apiKey.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read the API key by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    ApiKeyDto? apiKey = await _apiKeyService.ReadAsync(_apiKey.EntityId);
    Assert.NotNull(apiKey);
    Assert.Equal(_apiKey.EntityId, apiKey.Id);
  }

  [Fact(DisplayName = "It should replace an existing API key.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    Role admin = new(new UniqueName(Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
    Role guest = new(new UniqueName(Realm.UniqueNameSettings, "guest"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync([admin, guest]);

    _apiKey.AddRole(guest, ActorId);
    await _apiKeyRepository.SaveAsync(_apiKey);

    CreateOrReplaceApiKeyPayload payload = new()
    {
      Name = " External ",
      Description = "  This is the API key for external services.  ",
      ExpiresOn = DateTime.Now.AddMonths(3)
    };
    payload.CustomAttributes.Add(new CustomAttribute("manage_api", bool.TrueString));
    payload.Roles.Add("  AdmIn  ");

    CreateOrReplaceApiKeyResult result = await _apiKeyService.CreateOrReplaceAsync(payload, _apiKey.EntityId);
    Assert.False(result.Created);

    ApiKeyDto? apiKey = result.ApiKey;
    Assert.NotNull(apiKey);
    Assert.Equal(_apiKey.EntityId, apiKey.Id);
    Assert.Equal(_apiKey.Version + 3, apiKey.Version);
    Assert.Equal(Actor, apiKey.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, apiKey.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.Trim(), apiKey.Name);
    Assert.Equal(payload.Description.Trim(), apiKey.Description);
    Assert.NotNull(apiKey.ExpiresOn);
    Assert.Equal(payload.ExpiresOn.Value.AsUniversalTime(), apiKey.ExpiresOn.Value.AsUniversalTime());
    Assert.Equal(payload.CustomAttributes, apiKey.CustomAttributes);

    Assert.Single(apiKey.Roles);
    Assert.Contains(apiKey.Roles, r => r.Id == admin.EntityId);
  }

  [Fact(DisplayName = "It should replace an existing API key from reference.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    Role admin = new(new UniqueName(Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
    Role editor = new(new UniqueName(Realm.UniqueNameSettings, "editor"), ActorId, RoleId.NewId(Realm.Id));
    Role guest = new(new UniqueName(Realm.UniqueNameSettings, "guest"), ActorId, RoleId.NewId(Realm.Id));
    Role member = new(new UniqueName(Realm.UniqueNameSettings, "member"), ActorId, RoleId.NewId(Realm.Id));
    Role publisher = new(new UniqueName(Realm.UniqueNameSettings, "publisher"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync([admin, editor, guest, member, publisher]);

    _apiKey.AddRole(editor, ActorId);
    _apiKey.AddRole(guest, ActorId);
    _apiKey.AddRole(publisher, ActorId);

    long version = _apiKey.Version;

    Description description = new("  This is the administration API key.  ");
    _apiKey.Description = description;
    _apiKey.Update(ActorId);
    _apiKey.AddRole(member, ActorId);
    _apiKey.RemoveRole(guest, ActorId);
    await _apiKeyRepository.SaveAsync(_apiKey);

    CreateOrReplaceApiKeyPayload payload = new()
    {
      Name = $" {_apiKey.Name.Value} ",
      ExpiresOn = DateTime.Now.AddMonths(6)
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "Value"));
    payload.Roles.Add("  AdmIn  ");
    payload.Roles.Add("guest");
    payload.Roles.Add("publisher");

    CreateOrReplaceApiKeyResult result = await _apiKeyService.CreateOrReplaceAsync(payload, _apiKey.EntityId, version);
    Assert.False(result.Created);

    ApiKeyDto? apiKey = result.ApiKey;
    Assert.NotNull(apiKey);
    Assert.Equal(_apiKey.EntityId, apiKey.Id);
    Assert.Equal(_apiKey.Version + 3, apiKey.Version);
    Assert.Equal(Actor, apiKey.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, apiKey.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.Trim(), apiKey.Name);
    Assert.Equal(description.Value, apiKey.Description);
    Assert.NotNull(apiKey.ExpiresOn);
    Assert.Equal(payload.ExpiresOn.Value.AsUniversalTime(), apiKey.ExpiresOn.Value.AsUniversalTime());
    Assert.Equal(payload.CustomAttributes, apiKey.CustomAttributes);

    Assert.Equal(3, apiKey.Roles.Count);
    Assert.Contains(apiKey.Roles, r => r.Id == admin.EntityId);
    Assert.Contains(apiKey.Roles, r => r.Id == member.EntityId);
    Assert.Contains(apiKey.Roles, r => r.Id == publisher.EntityId);
  }

  [Fact(DisplayName = "It should return null when the API key cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _apiKeyService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_ApiKeys_When_Search_Then_CorrectResults()
  {
    ApiKey editor = new(_secret, new DisplayName("Editor"), ActorId, ApiKeyId.NewId(Realm.Id));
    ApiKey guest = new(_secret, new DisplayName("Guest"), ActorId, ApiKeyId.NewId(Realm.Id));
    ApiKey member = new(_secret, new DisplayName("Member"), ActorId, ApiKeyId.NewId(Realm.Id));
    await _apiKeyRepository.SaveAsync([editor, guest, member]);

    SearchApiKeysPayload payload = new()
    {
      Ids = [_apiKey.EntityId, editor.EntityId, member.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("%es%"), new SearchTerm("____or")], SearchOperator.Or),
      Sort = [new ApiKeySortOption(ApiKeySort.Name)],
      Skip = 1,
      Limit = 1
    };
    SearchResults<ApiKeyDto> results = await _apiKeyService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    ApiKeyDto apiKey = Assert.Single(results.Items);
    Assert.Equal(_apiKey.EntityId, apiKey.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (Expired).")]
  public async Task Given_Expired_When_Search_Then_CorrectResults()
  {
    ApiKey apiKey = new(_secret, new DisplayName("Default"), ActorId, ApiKeyId.NewId(Realm.Id))
    {
      ExpiresOn = DateTime.Now.AddDays(90)
    };
    apiKey.Update(ActorId);
    await _apiKeyRepository.SaveAsync(apiKey);

    SearchApiKeysPayload payload = new()
    {
      Status = new ApiKeyStatus(isExpired: true, DateTime.Now.AddYears(1))
    };
    SearchResults<ApiKeyDto> results = await _apiKeyService.SearchAsync(payload);

    Assert.Equal(1, results.Total);
    ApiKeyDto result = Assert.Single(results.Items);
    Assert.Equal(apiKey.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (HasAuthenticated).")]
  public async Task Given_HasAuthenticated_When_Search_Then_CorrectResults()
  {
    ApiKey apiKey = new(_secret, new DisplayName("Authenticated"), ActorId, ApiKeyId.NewId(Realm.Id));
    apiKey.Authenticate(_secretString, ActorId);
    await _apiKeyRepository.SaveAsync(apiKey);

    SearchApiKeysPayload payload = new()
    {
      HasAuthenticated = true
    };
    SearchResults<ApiKeyDto> results = await _apiKeyService.SearchAsync(payload);

    Assert.Equal(1, results.Total);
    ApiKeyDto result = Assert.Single(results.Items);
    Assert.Equal(apiKey.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (RoleId).")]
  public async Task Given_RoleId_When_Search_Then_CorrectResults()
  {
    Role admin = new(new UniqueName(base.Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync(admin);

    ApiKey apiKey = new(_secret, new DisplayName("Default"), ActorId, ApiKeyId.NewId(Realm.Id));
    apiKey.AddRole(admin, ActorId);
    await _apiKeyRepository.SaveAsync(apiKey);

    SearchApiKeysPayload payload = new()
    {
      RoleId = admin.EntityId
    };
    SearchResults<ApiKeyDto> results = await _apiKeyService.SearchAsync(payload);

    Assert.Equal(1, results.Total);
    ApiKeyDto result = Assert.Single(results.Items);
    Assert.Equal(apiKey.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (NotExpired).")]
  public async Task Given_NotExpired_When_Search_Then_CorrectResults()
  {
    ApiKey apiKey = new(_secret, new DisplayName("Default"), ActorId, ApiKeyId.NewId(Realm.Id))
    {
      ExpiresOn = DateTime.Now.AddDays(90)
    };
    apiKey.Update(ActorId);
    await _apiKeyRepository.SaveAsync(apiKey);

    SearchApiKeysPayload payload = new()
    {
      Status = new ApiKeyStatus(isExpired: false)
    };
    SearchResults<ApiKeyDto> results = await _apiKeyService.SearchAsync(payload);

    Assert.Equal(1, results.Total);
    ApiKeyDto result = Assert.Single(results.Items);
    Assert.Equal(apiKey.EntityId, result.Id);
  }

  [Fact(DisplayName = "It should update an existing API key.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Role admin = new(new UniqueName(Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
    Role editor = new(new UniqueName(Realm.UniqueNameSettings, "editor"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync([admin, editor]);

    _apiKey.AddRole(admin, ActorId);
    _apiKey.AddRole(editor, ActorId);
    await _apiKeyRepository.SaveAsync(_apiKey);

    UpdateApiKeyPayload payload = new()
    {
      Name = "Default"
    };
    payload.CustomAttributes.Add(new CustomAttribute("manage_api", $"  {bool.TrueString}  "));
    payload.CustomAttributes.Add(new CustomAttribute("removed", string.Empty));
    payload.Roles.Add(new RoleChange("  ADMIN  ", CollectionAction.Add));
    payload.Roles.Add(new RoleChange("editor", CollectionAction.Remove));

    ApiKeyDto? apiKey = await _apiKeyService.UpdateAsync(_apiKey.EntityId, payload);
    Assert.NotNull(apiKey);

    Assert.Equal(_apiKey.EntityId, apiKey.Id);
    Assert.Equal(_apiKey.Version + 2, apiKey.Version);
    Assert.Equal(Actor, apiKey.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, apiKey.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.Trim(), apiKey.Name);
    Assert.Equal(_apiKey.Description?.Value, apiKey.Description);
    Assert.Equal(_apiKey.ExpiresOn, apiKey.ExpiresOn);
    Assert.Single(apiKey.CustomAttributes);
    Assert.Contains(apiKey.CustomAttributes, c => c.Key == "manage_api" && c.Value == bool.TrueString);

    Assert.Single(apiKey.Roles);
    Assert.Contains(apiKey.Roles, r => r.Id == admin.EntityId);
  }
}
