using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Search;
using RoleDto = Krakenar.Contracts.Roles.Role;

namespace Krakenar.Core.Roles;

public interface IRoleQuerier
{
  Task<RoleId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<IReadOnlyDictionary<Guid, string>> ListUniqueNameByIdsAsync(CancellationToken cancellationToken = default);

  Task<RoleDto> ReadAsync(Role role, CancellationToken cancellationToken = default);
  Task<RoleDto?> ReadAsync(RoleId id, CancellationToken cancellationToken = default);
  Task<RoleDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RoleDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<RoleDto>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken = default);
}
