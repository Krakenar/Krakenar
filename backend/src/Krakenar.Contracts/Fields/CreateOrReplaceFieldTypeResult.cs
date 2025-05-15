namespace Krakenar.Contracts.Fields;

public record CreateOrReplaceFieldTypeResult
{
  public FieldType? FieldType { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceFieldTypeResult()
  {
  }

  public CreateOrReplaceFieldTypeResult(FieldType? fieldType, bool created)
  {
    FieldType = fieldType;
    Created = created;
  }
}
