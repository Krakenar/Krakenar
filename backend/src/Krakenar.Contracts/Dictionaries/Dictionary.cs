using Krakenar.Contracts.Localization;

namespace Krakenar.Contracts.Dictionaries;

public class Dictionary : Aggregate
{
  public Language Language { get; set; } = new();

  public int EntryCount { get; set; }
  public List<DictionaryEntry> Entries { get; set; } = [];

  public override string ToString() => $"{Language.Locale} | {base.ToString()}";
}
