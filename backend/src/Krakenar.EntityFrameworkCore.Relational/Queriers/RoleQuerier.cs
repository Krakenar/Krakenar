using Krakenar.Core;
using Krakenar.Core.Roles;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class RoleQuerier : IRoleQuerier // TODO(fpion): implement
{
  public virtual Task<RoleId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public virtual Task<RoleDto> ReadAsync(Role role, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<RoleDto?> ReadAsync(RoleId id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<RoleDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
  public virtual Task<RoleDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
