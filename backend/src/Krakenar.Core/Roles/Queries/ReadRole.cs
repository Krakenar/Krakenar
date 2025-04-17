using Krakenar.Contracts;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles.Queries;

public record ReadRole(Guid? Id, string? UniqueName) : IQuery<RoleDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadRoleHandler : IQueryHandler<ReadRole, RoleDto?>
{
  protected virtual IRoleQuerier RoleQuerier { get; }

  public ReadRoleHandler(IRoleQuerier roleQuerier)
  {
    RoleQuerier = roleQuerier;
  }

  public virtual async Task<RoleDto?> HandleAsync(ReadRole query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, RoleDto> roles = new(capacity: 2);

    if (query.Id.HasValue)
    {
      RoleDto? role = await RoleQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (role is not null)
      {
        roles[role.Id] = role;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      RoleDto? role = await RoleQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (role is not null)
      {
        roles[role.Id] = role;
      }
    }

    if (roles.Count > 1)
    {
      throw TooManyResultsException<RoleDto>.ExpectedSingle(roles.Count);
    }

    return roles.SingleOrDefault().Value;
  }
}
