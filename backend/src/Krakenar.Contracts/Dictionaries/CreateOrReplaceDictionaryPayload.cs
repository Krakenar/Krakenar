namespace Krakenar.Contracts.Dictionaries;

public record CreateOrReplaceDictionaryPayload
{
  public Guid LanguageId { get; set; }

  public List<DictionaryEntry> Entries { get; set; } = [];

  public CreateOrReplaceDictionaryPayload() : this(Guid.Empty)
  {
  }

  public CreateOrReplaceDictionaryPayload(Guid languageId)
  {
    LanguageId = languageId;
  }
}
