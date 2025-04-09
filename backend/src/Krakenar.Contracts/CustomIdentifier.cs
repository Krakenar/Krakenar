namespace Krakenar.Contracts;

public record CustomIdentifier
{
  public string Key { get; set; }
  public string Value { get; set; }

  public CustomIdentifier() : this(string.Empty, string.Empty)
  {
  }

  public CustomIdentifier(KeyValuePair<string, string> customIdentifier) : this(customIdentifier.Key, customIdentifier.Value)
  {
  }

  public CustomIdentifier(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
