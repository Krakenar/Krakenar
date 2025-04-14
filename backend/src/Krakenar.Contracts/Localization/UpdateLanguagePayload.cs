namespace Krakenar.Contracts.Localization;

public record UpdateLanguagePayload
{
  public string? Locale { get; set; }
}
