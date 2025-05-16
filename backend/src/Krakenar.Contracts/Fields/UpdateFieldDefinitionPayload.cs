namespace Krakenar.Contracts.Fields;

public record UpdateFieldDefinitionPayload
{
  public bool? IsInvariant { get; set; }
  public bool? IsRequired { get; set; }
  public bool? IsIndexed { get; set; }
  public bool? IsUnique { get; set; }

  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }
  public Change<string>? Placeholder { get; set; }
}
