using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.ApiKeys;
using Krakenar.Core.Passwords;
using Krakenar.Core.Roles;
using Krakenar.Core.Users;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Role = Krakenar.Core.Roles.Role;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Roles;

[Trait(Traits.Category, Categories.Integration)]
public class RoleIntegrationTests : IntegrationTests
{
  private readonly IApiKeyRepository _apiKeyRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly IRoleRepository _roleRepository;
  private readonly IRoleService _roleService;
  private readonly IUserRepository _userRepository;

  private readonly Role _role;

  public RoleIntegrationTests() : base()
  {
    _apiKeyRepository = ServiceProvider.GetRequiredService<IApiKeyRepository>();
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
    _roleService = ServiceProvider.GetRequiredService<IRoleService>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

    _role = new(new UniqueName(Realm.UniqueNameSettings, "admin"), ActorId, RoleId.NewId(Realm.Id));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _roleRepository.SaveAsync(_role);
  }

  [Fact(DisplayName = "It should create a new role.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created()
  {
    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = "editor",
      DisplayName = " Editor ",
      Description = "  This is the role for document editors.  "
    };
    payload.CustomAttributes.Add(new CustomAttribute("edit_documents", bool.TrueString));
    payload.CustomAttributes.Add(new CustomAttribute("publish_documents", bool.FalseString));

    Guid id = Guid.NewGuid();
    CreateOrReplaceRoleResult result = await _roleService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    RoleDto? role = result.Role;
    Assert.NotNull(role);
    Assert.Equal(id, role.Id);
    Assert.Equal(2, role.Version);
    Assert.Equal(Actor, role.CreatedBy);
    Assert.Equal(DateTime.UtcNow, role.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, role.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, role.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, role.Realm);
    Assert.Equal(payload.UniqueName, role.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), role.DisplayName);
    Assert.Equal(payload.Description.Trim(), role.Description);
    Assert.Equal(payload.CustomAttributes, role.CustomAttributes);
  }

  [Fact(DisplayName = "It should delete the role.")]
  public async Task Given_Role_When_Delete_Then_Deleted()
  {
    Password secret = _passwordManager.GenerateBase64(XApiKey.SecretLength, out _);
    ApiKey apiKey = new(secret, new DisplayName("Test API Key"), ActorId, ApiKeyId.NewId(Realm.Id));
    apiKey.AddRole(_role, ActorId);
    await _apiKeyRepository.SaveAsync(apiKey);

    User user = new(new UniqueName(Realm.UniqueNameSettings, Faker.Person.UserName), password: null, ActorId, UserId.NewId(Realm.Id));
    user.AddRole(_role, ActorId);
    await _userRepository.SaveAsync(user);

    RoleDto? role = await _roleService.DeleteAsync(_role.EntityId);
    Assert.NotNull(role);
    Assert.Equal(_role.EntityId, role.Id);

    Assert.Empty(await KrakenarContext.Roles.AsNoTracking().Where(x => x.StreamId == _role.Id.Value).ToArrayAsync());

    apiKey = (await _apiKeyRepository.LoadAsync(apiKey.Id))!;
    Assert.NotNull(apiKey);
    Assert.Empty(apiKey.Roles);

    user = (await _userRepository.LoadAsync(user.Id))!;
    Assert.NotNull(user);
    Assert.Empty(user.Roles);
  }

  [Fact(DisplayName = "It should read the role by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    RoleDto? role = await _roleService.ReadAsync(_role.EntityId);
    Assert.NotNull(role);
    Assert.Equal(_role.EntityId, role.Id);
  }

  [Fact(DisplayName = "It should read the role by unique name.")]
  public async Task Given_UniqueName_When_Read_Then_Found()
  {
    RoleDto? role = await _roleService.ReadAsync(id: null, _role.UniqueName.Value);
    Assert.NotNull(role);
    Assert.Equal(_role.EntityId, role.Id);
  }

  [Fact(DisplayName = "It should replace an existing role.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = "admin",
      DisplayName = " Administrator ",
      Description = "  This is the administration role.  "
    };
    payload.CustomAttributes.Add(new CustomAttribute("manage_api", bool.TrueString));

    CreateOrReplaceRoleResult result = await _roleService.CreateOrReplaceAsync(payload, _role.EntityId);
    Assert.False(result.Created);

    RoleDto? role = result.Role;
    Assert.NotNull(role);
    Assert.Equal(_role.EntityId, role.Id);
    Assert.Equal(_role.Version + 1, role.Version);
    Assert.Equal(Actor, role.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, role.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueName, role.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), role.DisplayName);
    Assert.Equal(payload.Description.Trim(), role.Description);
    Assert.Equal(payload.CustomAttributes, role.CustomAttributes);
  }

  [Fact(DisplayName = "It should replace an existing role from reference.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    long version = _role.Version;

    Description description = new("  This is the administration role.  ");
    _role.Description = description;
    _role.Update(ActorId);
    await _roleRepository.SaveAsync(_role);

    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = "admin",
      DisplayName = " Administrator "
    };
    payload.CustomAttributes.Add(new CustomAttribute("Key", "Value"));

    CreateOrReplaceRoleResult result = await _roleService.CreateOrReplaceAsync(payload, _role.EntityId, version);
    Assert.False(result.Created);

    RoleDto? role = result.Role;
    Assert.NotNull(role);
    Assert.Equal(_role.EntityId, role.Id);
    Assert.Equal(_role.Version + 1, role.Version);
    Assert.Equal(Actor, role.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, role.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueName, role.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), role.DisplayName);
    Assert.Equal(description.Value, role.Description);
    Assert.Equal(payload.CustomAttributes, role.CustomAttributes);
  }

  [Fact(DisplayName = "It should return null when the role cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _roleService.ReadAsync(Guid.Empty, "not-found"));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Roles_When_Search_Then_CorrectResults()
  {
    Role editor = new(new UniqueName(Realm.UniqueNameSettings, "editor"), ActorId, RoleId.NewId(Realm.Id));
    Role guest = new(new UniqueName(Realm.UniqueNameSettings, "guest"), ActorId, RoleId.NewId(Realm.Id));
    Role member = new(new UniqueName(Realm.UniqueNameSettings, "member"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync([editor, guest, member]);

    SearchRolesPayload payload = new()
    {
      Ids = [_role.EntityId, editor.EntityId, guest.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("a%"), new SearchTerm("%r")], SearchOperator.Or),
      Sort = [new RoleSortOption(RoleSort.UniqueName, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    SearchResults<RoleDto> results = await _roleService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    RoleDto role = Assert.Single(results.Items);
    Assert.Equal(_role.EntityId, role.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple roles were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    Role role = new(new UniqueName(Realm.UniqueNameSettings, "editor"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync(role);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<RoleDto>>(async () => await _roleService.ReadAsync(_role.EntityId, role.UniqueName.Value));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when a unique name conflict occurs.")]
  public async Task Given_UniqueNameConflict_When_CreateOrReplace_Then_UniqueNameAlreadyUsedException()
  {
    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = _role.UniqueName.Value
    };
    Guid id = Guid.NewGuid();
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _roleService.CreateOrReplaceAsync(payload, id));

    Assert.Equal(_role.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("Role", exception.EntityType);
    Assert.Equal(id, exception.EntityId);
    Assert.Equal(_role.EntityId, exception.ConflictId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing role.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateRolePayload payload = new()
    {
      DisplayName = new Contracts.Change<string>(" Administrator ")
    };
    payload.CustomAttributes.Add(new CustomAttribute("manage_api", $"  {bool.TrueString}  "));
    payload.CustomAttributes.Add(new CustomAttribute("removed", string.Empty));
    RoleDto? role = await _roleService.UpdateAsync(_role.EntityId, payload);
    Assert.NotNull(role);

    Assert.Equal(_role.EntityId, role.Id);
    Assert.Equal(_role.Version + 1, role.Version);
    Assert.Equal(Actor, role.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, role.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(_role.UniqueName.Value, role.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), role.DisplayName);
    Assert.Equal(_role.Description?.Value, role.Description);
    Assert.Single(role.CustomAttributes);
    Assert.Contains(role.CustomAttributes, c => c.Key == "manage_api" && c.Value == bool.TrueString);
  }
}
