using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Users;

public record SearchUsersPayload : SearchPayload
{
  public bool? HasPassword { get; set; }
  public bool? IsDisabled { get; set; }
  public bool? IsConfirmed { get; set; }
  public bool? HasAuthenticated { get; set; }
  public Guid? RoleId { get; set; }

  public new List<UserSortOption> Sort { get; set; } = [];
}
