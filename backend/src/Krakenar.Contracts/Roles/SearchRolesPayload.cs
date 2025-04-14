using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Roles;

public record SearchRolesPayload : SearchPayload
{
  public new List<RoleSortOption> Sort { get; set; } = [];
}
