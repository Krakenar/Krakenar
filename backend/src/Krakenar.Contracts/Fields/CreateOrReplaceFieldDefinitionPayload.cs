namespace Krakenar.Contracts.Fields;

public record CreateOrReplaceFieldDefinitionPayload
{
  public string FieldType { get; set; }

  public bool IsInvariant { get; set; }
  public bool IsRequired { get; set; }
  public bool IsIndexed { get; set; }
  public bool IsUnique { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
  public string? Placeholder { get; set; }

  public CreateOrReplaceFieldDefinitionPayload() : this(string.Empty, string.Empty)
  {
  }

  public CreateOrReplaceFieldDefinitionPayload(string uniqueName, string fieldType)
  {
    FieldType = fieldType;

    UniqueName = uniqueName;
  }
}
