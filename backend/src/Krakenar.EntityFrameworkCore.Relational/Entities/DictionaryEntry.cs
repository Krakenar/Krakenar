using Logitar;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class DictionaryEntry
{
  public const int ValueShortenedLength = byte.MaxValue;

  public int DictionaryEntryId { get; private set; }

  public Dictionary? Dictionary { get; private set; }
  public int DictionaryId { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public string ValueShortened
  {
    get => Value.Truncate(ValueShortenedLength);
    private set { }
  }

  public DictionaryEntry(Dictionary dictionary, Core.Identifier key, string value)
  {
    Dictionary = dictionary;
    DictionaryId = dictionary.DictionaryId;

    Key = key.Value;
    Value = value;
  }

  private DictionaryEntry()
  {
  }

  public override bool Equals(object? obj) => obj is DictionaryEntry entry && entry.DictionaryId == DictionaryId && entry.Key == Key;
  public override int GetHashCode() => HashCode.Combine(DictionaryId, Key);
  public override string ToString() => $"{GetType()} (DictionaryId={DictionaryId}, Key={Key})";
}
