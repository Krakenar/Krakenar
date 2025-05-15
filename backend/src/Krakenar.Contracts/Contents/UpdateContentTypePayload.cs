namespace Krakenar.Contracts.Contents;

public record UpdateContentTypePayload
{
  public bool? IsInvariant { get; set; }

  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }
}
