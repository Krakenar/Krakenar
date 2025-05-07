namespace Krakenar.Contracts.Messages;

public record Variable
{
  public string Key { get; set; }
  public string Value { get; set; }

  public Variable() : this(string.Empty, string.Empty)
  {
  }

  public Variable(KeyValuePair<string, string> variable) : this(variable.Key, variable.Value)
  {
  }

  public Variable(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
