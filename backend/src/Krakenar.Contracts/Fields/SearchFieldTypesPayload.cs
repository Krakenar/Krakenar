using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Fields;

public record SearchFieldTypesPayload : SearchPayload
{
  public DataType? DataType { get; set; }

  public new List<FieldTypeSortOption> Sort { get; set; } = [];
}
