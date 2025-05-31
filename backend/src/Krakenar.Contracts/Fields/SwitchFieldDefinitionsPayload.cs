namespace Krakenar.Contracts.Fields;

public record SwitchFieldDefinitionsPayload
{
  public List<string> Fields { get; set; } = [];

  public SwitchFieldDefinitionsPayload(IEnumerable<string>? fields = null)
  {
    if (fields is not null)
    {
      Fields.AddRange(fields);
    }
  }
}
