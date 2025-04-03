namespace Krakenar.Contracts;

public record Error
{
  public string Code { get; set; }
  public string Message { get; set; }
  public Dictionary<string, object?> Data { get; set; } = [];

  public Error() : this(string.Empty, string.Empty)
  {
  }

  public Error(string code, string message, IEnumerable<KeyValuePair<string, object?>>? data = null)
  {
    Code = code;
    Message = message;

    if (data is not null)
    {
      foreach (KeyValuePair<string, object?> pair in data)
      {
        Data[pair.Key] = pair.Value;
      }
    }
  }
}
