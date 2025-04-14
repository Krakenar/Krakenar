namespace Krakenar.Contracts.Localization;

public record CreateOrReplaceLanguagePayload
{
  public string Locale { get; set; } = string.Empty;
}
