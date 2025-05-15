using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Contents;

public record SearchContentTypesPayload : SearchPayload
{
  public bool? IsInvariant { get; set; }

  public new List<ContentTypeSortOption> Sort { get; set; } = [];
}
