namespace Krakenar.Contracts.Localization;

public record CreateOrReplaceLanguageResult
{
  public Language? Language { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceLanguageResult()
  {
  }

  public CreateOrReplaceLanguageResult(Language? language, bool created)
  {
    Language = language;
    Created = created;
  }
}
