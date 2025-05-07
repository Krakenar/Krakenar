using Logitar;

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
      foreach (KeyValuePair<string, object?> item in data)
      {
        Data[item.Key] = item.Value;
      }
    }
  }

  public Error(Exception exception)
  {
    Code = exception.GetErrorCode();
    Message = exception.Message;

    Data[nameof(exception.HelpLink)] = exception.HelpLink;
    Data[nameof(exception.HResult)] = exception.HResult;
    Data[nameof(exception.InnerException)] = exception.InnerException is null ? null : new Error(exception.InnerException);
    Data[nameof(exception.Source)] = exception.Source;
    Data[nameof(exception.StackTrace)] = exception.StackTrace;
    Data[nameof(exception.TargetSite)] = exception.TargetSite?.ToString();

    int capacity = exception.Data.Count;
    if (capacity > 0)
    {
      JsonSerializerOptions serializerOptions = new();
      serializerOptions.Converters.Add(new JsonStringEnumConverter());

      Dictionary<string, object?> data = new(capacity);
      foreach (DictionaryEntry entry in exception.Data)
      {
        try
        {
          string key = JsonSerializer.Serialize(entry.Key, serializerOptions);
          _ = JsonSerializer.Serialize(entry.Value, serializerOptions);
          data[key] = entry.Value;
        }
        catch (Exception)
        {
        }
      }
      Data[nameof(exception.Data)] = data;
    }
  }
}
