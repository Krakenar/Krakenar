namespace Krakenar.Contracts.Dictionaries;

public record UpdateDictionaryPayload
{
  public string? Language { get; set; }

  public List<DictionaryEntry> Entries { get; set; } = [];
}
