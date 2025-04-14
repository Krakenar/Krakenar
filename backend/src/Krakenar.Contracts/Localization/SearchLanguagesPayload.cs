using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Localization;

public record SearchLanguagesPayload : SearchPayload
{
  public new List<LanguageSortOption> Sort { get; set; } = [];
}
