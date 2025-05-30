using Logitar;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class LogException
{
  public long LogExceptionId { get; private set; }
  public Guid Id { get; private set; }

  public Log? Log { get; private set; }
  public long LogId { get; private set; }
  public Guid LogUid { get; private set; }

  public string Type { get; private set; } = string.Empty;
  public string Message { get; private set; } = string.Empty;

  public int HResult { get; private set; }
  public string? HelpLink { get; private set; }
  public string? Source { get; private set; }
  public string? StackTrace { get; private set; }
  public string? TargetSite { get; private set; }

  public string? Data { get; private set; }

  public LogException(Log log, Exception exception, JsonSerializerOptions? serializerOptions = null)
  {
    Id = Guid.NewGuid();

    Log = log;
    LogId = log.LogId;
    LogUid = log.Id;

    Type = exception.GetType().GetNamespaceQualifiedName();
    Message = exception.Message;

    HResult = exception.HResult;
    HelpLink = exception.HelpLink;
    Source = exception.Source;
    StackTrace = exception.StackTrace;
    TargetSite = exception.TargetSite?.ToString();

    Dictionary<string, string> data = new(capacity: exception.Data.Count);
    foreach (object key in exception.Data.Keys)
    {
      try
      {
        object? value = exception.Data[key];
        if (value != null)
        {
          string serializedKey = JsonSerializer.Serialize(key, key.GetType(), serializerOptions).Trim('"');
          string serializedValue = JsonSerializer.Serialize(value, value.GetType(), serializerOptions).Trim('"');
          data[serializedKey] = serializedValue;
        }
      }
      catch (Exception)
      {
      }
    }
    Data = data.Count < 1 ? null : JsonSerializer.Serialize(data);
  }

  private LogException()
  {
  }
}
