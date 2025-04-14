using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Realms;

public record SearchRealmsPayload : SearchPayload
{
  public new List<RealmSortOption> Sort { get; set; } = [];
}
