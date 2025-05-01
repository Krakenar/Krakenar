namespace Krakenar.Contracts.Dictionaries;

public record UpdateDictionaryPayload
{
  public Guid? LanguageId { get; set; }

  public List<DictionaryEntry> Entries { get; set; } = [];
}
