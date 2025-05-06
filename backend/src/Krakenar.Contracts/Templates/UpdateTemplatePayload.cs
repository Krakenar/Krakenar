namespace Krakenar.Contracts.Templates;

public record UpdateTemplatePayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public string? Subject { get; set; }
  public Content? Content { get; set; }
}
