using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using Krakenar.Core.Roles.Commands;
using Krakenar.Core.Roles.Queries;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles;

public class RoleService : IRoleService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public RoleService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<CreateOrReplaceRoleResult> CreateOrReplaceAsync(CreateOrReplaceRolePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceRole command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<RoleDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteRole command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<RoleDto?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadRole query = new(id, uniqueName);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<RoleDto>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken)
  {
    SearchRoles query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<RoleDto?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken)
  {
    UpdateRole command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
