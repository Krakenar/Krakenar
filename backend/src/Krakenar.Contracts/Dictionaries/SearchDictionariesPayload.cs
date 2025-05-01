using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Dictionaries;

public record SearchDictionariesPayload : SearchPayload
{
  public new List<DictionarySortOption> Sort { get; set; } = [];
}
