using Krakenar.Client.Roles;
using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class RoleClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public RoleClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Roles should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient httpClient = new();
    KrakenarSettings.Realm = Realm.UniqueSlug;
    RoleClient roles = new(httpClient, KrakenarSettings);

    Guid id = Guid.Parse("867002d9-68ae-409c-9bb3-ba1cbc14a7ff");
    Role? role = await roles.ReadAsync(id, uniqueName: null, _cancellationToken);

    CreateOrReplaceRolePayload createOrReplaceRole = new("admin");
    CreateOrReplaceRoleResult roleResult = await roles.CreateOrReplaceAsync(createOrReplaceRole, id, version: null, _cancellationToken);
    Assert.Equal(roleResult.Created, role is null);
    role = roleResult.Role;
    Assert.NotNull(role);
    Assert.Equal(id, role.Id);
    Assert.Equal(createOrReplaceRole.UniqueName, role.UniqueName);

    UpdateRolePayload updateRole = new()
    {
      DisplayName = new Change<string>(" Administrator "),
      CustomAttributes = [new CustomAttribute("manage_api", bool.TrueString)]
    };
    role = await roles.UpdateAsync(id, updateRole, _cancellationToken);
    Assert.NotNull(role);
    Assert.Equal(createOrReplaceRole.UniqueName, role.UniqueName);
    Assert.Equal(updateRole.DisplayName.Value?.Trim(), role.DisplayName);
    Assert.Equal(updateRole.CustomAttributes, role.CustomAttributes);

    role = await roles.ReadAsync(id: null, role.UniqueName, _cancellationToken);
    Assert.NotNull(role);
    Assert.Equal(id, role.Id);

    SearchRolesPayload searchRoles = new()
    {
      Ids = [role.Id],
      Search = new TextSearch([new SearchTerm("%tor")])
    };
    SearchResults<Role> results = await roles.SearchAsync(searchRoles, _cancellationToken);
    Assert.Equal(1, results.Total);
    role = Assert.Single(results.Items);
    Assert.Equal(id, role.Id);
  }
}
