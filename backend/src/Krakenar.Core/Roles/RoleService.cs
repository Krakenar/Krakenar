using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Core.Roles.Commands;
using Krakenar.Core.Roles.Queries;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles;

public class RoleService : IRoleService
{
  protected virtual ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult> CreateOrReplaceRole { get; }
  protected virtual ICommandHandler<DeleteRole, RoleDto?> DeleteRole { get; }
  protected virtual IQueryHandler<ReadRole, RoleDto?> ReadRole { get; }
  protected virtual IQueryHandler<SearchRoles, SearchResults<RoleDto>> SearchRoles { get; }
  protected virtual ICommandHandler<UpdateRole, RoleDto?> UpdateRole { get; }

  public RoleService(
    ICommandHandler<CreateOrReplaceRole, CreateOrReplaceRoleResult> createOrReplaceRole,
    ICommandHandler<DeleteRole, RoleDto?> deleteRole,
    IQueryHandler<ReadRole, RoleDto?> readRole,
    IQueryHandler<SearchRoles, SearchResults<RoleDto>> searchRoles,
    ICommandHandler<UpdateRole, RoleDto?> updateRole)
  {
    CreateOrReplaceRole = createOrReplaceRole;
    DeleteRole = deleteRole;
    ReadRole = readRole;
    SearchRoles = searchRoles;
    UpdateRole = updateRole;
  }

  public virtual async Task<CreateOrReplaceRoleResult> CreateOrReplaceAsync(CreateOrReplaceRolePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRole command = new(id, payload, version);
    return await CreateOrReplaceRole.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<RoleDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteRole command = new(id);
    return await DeleteRole.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<RoleDto?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadRole query = new(id, uniqueName);
    return await ReadRole.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<RoleDto>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    SearchRoles query = new(payload);
    return await SearchRoles.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<RoleDto?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    UpdateRole command = new(id, payload);
    return await UpdateRole.HandleAsync(command, cancellationToken);
  }
}
