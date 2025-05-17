namespace Krakenar.Contracts.Fields;

public record FieldValuePayload
{
  public string Field { get; set; }
  public string Value { get; set; }

  public FieldValuePayload() : this(string.Empty, string.Empty)
  {
  }

  public FieldValuePayload(KeyValuePair<string, string> customAttribute) : this(customAttribute.Key, customAttribute.Value)
  {
  }

  public FieldValuePayload(string field, string value)
  {
    Field = field;
    Value = value;
  }
}
