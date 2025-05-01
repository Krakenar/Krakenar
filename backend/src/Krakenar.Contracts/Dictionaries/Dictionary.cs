using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Realms;

namespace Krakenar.Contracts.Dictionaries;

public class Dictionary : Aggregate
{
  public Realm? Realm { get; set; }

  public Language Language { get; set; } = new();

  public int EntryCount { get; set; }
  public List<DictionaryEntry> Entries { get; set; } = [];

  public override string ToString() => $"{Language.Locale} | {base.ToString()}";
}
