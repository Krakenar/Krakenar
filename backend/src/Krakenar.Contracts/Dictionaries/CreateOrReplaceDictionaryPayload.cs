namespace Krakenar.Contracts.Dictionaries;

public record CreateOrReplaceDictionaryPayload
{
  public string Language { get; set; } = string.Empty;

  public List<DictionaryEntry> Entries { get; set; } = [];

  public CreateOrReplaceDictionaryPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceDictionaryPayload(string language)
  {
    Language = language;
  }
}
