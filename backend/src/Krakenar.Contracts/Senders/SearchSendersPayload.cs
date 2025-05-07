using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Senders;

public record SearchSendersPayload : SearchPayload
{
  public SenderKind? Kind { get; set; }
  public SenderProvider? Provider { get; set; }

  public new List<SenderSortOption> Sort { get; set; } = [];
}
