using Krakenar.Contracts;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Roles;
using Krakenar.Core.Roles.Commands;
using Krakenar.Core.Roles.Queries;
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
  private readonly IRoleRepository _roleRepository;
  private readonly IUserRepository _userRepository;

  private readonly ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult> _createOrReplaceRole;
  private readonly ICommandHandler<DeleteRole, RoleDto?> _deleteRole;
  private readonly IQueryHandler<ReadRole, RoleDto?> _readRole;
  private readonly IQueryHandler<SearchRoles, SearchResults<RoleDto>> _searchRoles;
  private readonly ICommandHandler<UpdateRole, RoleDto?> _updateRole;

  private readonly Role _role;

  public RoleIntegrationTests() : base()
  {
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

    _createOrReplaceRole = ServiceProvider.GetRequiredService<ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult>>();
    _deleteRole = ServiceProvider.GetRequiredService<ICommandHandler<DeleteRole, RoleDto?>>();
    _readRole = ServiceProvider.GetRequiredService<IQueryHandler<ReadRole, RoleDto?>>();
    _searchRoles = ServiceProvider.GetRequiredService<IQueryHandler<SearchRoles, SearchResults<RoleDto>>>();
    _updateRole = ServiceProvider.GetRequiredService<ICommandHandler<UpdateRole, RoleDto?>>();

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

    CreateOrReplaceRole command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceRoleResult result = await _createOrReplaceRole.HandleAsync(command);
    Assert.True(result.Created);

    RoleDto? role = result.Role;
    Assert.NotNull(role);
    Assert.Equal(command.Id, role.Id);
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
    User user = new(new UniqueName(Realm.UniqueNameSettings, Faker.Person.UserName), password: null, ActorId, UserId.NewId(Realm.Id));
    user.AddRole(_role, ActorId);
    await _userRepository.SaveAsync(user);

    DeleteRole command = new(_role.EntityId);
    RoleDto? role = await _deleteRole.HandleAsync(command);
    Assert.NotNull(role);
    Assert.Equal(command.Id, role.Id);

    Assert.Empty(await KrakenarContext.Roles.AsNoTracking().Where(x => x.StreamId == _role.Id.Value).ToArrayAsync());

    user = (await _userRepository.LoadAsync(user.Id))!;
    Assert.NotNull(user);
    Assert.Empty(user.Roles);
  }

  [Fact(DisplayName = "It should read the role by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    ReadRole query = new(_role.EntityId, UniqueName: null);
    RoleDto? role = await _readRole.HandleAsync(query);
    Assert.NotNull(role);
    Assert.Equal(query.Id, role.Id);
  }

  [Fact(DisplayName = "It should read the role by unique name.")]
  public async Task Given_UniqueName_When_Read_Then_Found()
  {
    ReadRole query = new(Id: null, _role.UniqueName.Value);
    RoleDto? role = await _readRole.HandleAsync(query);
    Assert.NotNull(role);
    Assert.Equal(_role.EntityId, role.Id);
  }

  [Fact(DisplayName = "It should replace an existing role.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceRolePayload payload = new()
    {
      UniqueName = "admin",
      DisplayName = " Administrator ",
      Description = "  This is the administration role.  "
    };
    payload.CustomAttributes.Add(new CustomAttribute("manage_api", bool.TrueString));

    CreateOrReplaceRole command = new(_role.EntityId, payload, Version: null);
    CreateOrReplaceRoleResult result = await _createOrReplaceRole.HandleAsync(command);
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
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
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

    CreateOrReplaceRole command = new(_role.EntityId, payload, version);
    CreateOrReplaceRoleResult result = await _createOrReplaceRole.HandleAsync(command);
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
    ReadRole query = new(Guid.Empty, "not-found");
    Assert.Null(await _readRole.HandleAsync(query));
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
    SearchRoles command = new(payload);
    SearchResults<RoleDto> results = await _searchRoles.HandleAsync(command);
    Assert.Equal(2, results.Total);

    RoleDto role = Assert.Single(results.Items);
    Assert.Equal(_role.EntityId, role.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple roles were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    Role role = new(new UniqueName(Realm.UniqueNameSettings, "editor"), ActorId, RoleId.NewId(Realm.Id));
    await _roleRepository.SaveAsync(role);

    ReadRole query = new(_role.EntityId, role.UniqueName.Value);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<RoleDto>>(async () => await _readRole.HandleAsync(query));
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
    CreateOrReplaceRole command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _createOrReplaceRole.HandleAsync(command));

    Assert.Equal(_role.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("Role", exception.EntityType);
    Assert.Equal(command.Id, exception.EntityId);
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
    UpdateRole command = new(_role.EntityId, payload);
    RoleDto? role = await _updateRole.HandleAsync(command);
    Assert.NotNull(role);

    Assert.Equal(command.Id, role.Id);
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
