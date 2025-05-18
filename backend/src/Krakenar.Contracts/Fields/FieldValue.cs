namespace Krakenar.Contracts.Fields;

public record FieldValue
{
  public Guid Id { get; set; }
  public string Value { get; set; }

  public FieldValue() : this(Guid.Empty, string.Empty)
  {
  }

  public FieldValue(KeyValuePair<Guid, string> customAttribute) : this(customAttribute.Key, customAttribute.Value)
  {
  }

  public FieldValue(Guid id, string value)
  {
    Id = id;
    Value = value;
  }
}
