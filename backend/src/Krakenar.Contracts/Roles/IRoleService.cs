using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Roles;

public interface IRoleService
{
  Task<CreateOrReplaceRoleResult> CreateOrReplaceAsync(CreateOrReplaceRolePayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<Role?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Role?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Role>> SearchAsync(SearchRolesPayload payload, CancellationToken cancellationToken = default);
  Task<Role?> UpdateAsync(Guid id, UpdateRolePayload payload, CancellationToken cancellationToken = default);
}
