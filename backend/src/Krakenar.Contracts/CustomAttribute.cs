namespace Krakenar.Contracts;

public record CustomAttribute
{
  public string Key { get; set; }
  public string Value { get; set; }

  public CustomAttribute() : this(string.Empty, string.Empty)
  {
  }

  public CustomAttribute(KeyValuePair<string, string> customAttribute) : this(customAttribute.Key, customAttribute.Value)
  {
  }

  public CustomAttribute(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
