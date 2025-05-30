using Logitar;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Krakenar.MongoDB;

public sealed class LogException
{
  [BsonRepresentation(BsonType.String)]
  public Guid UniqueId { get; private set; }

  public string Type { get; private set; } = string.Empty;
  public string Message { get; private set; } = string.Empty;

  public int HResult { get; private set; }
  public string? HelpLink { get; private set; }
  public string? Source { get; private set; }
  public string? StackTrace { get; private set; }
  public string? TargetSite { get; private set; }

  public Dictionary<string, string> Data { get; private set; } = [];

  public LogException(Exception exception, JsonSerializerOptions? serializerOptions = null)
  {
    UniqueId = Guid.NewGuid();

    Type = exception.GetType().GetNamespaceQualifiedName();
    Message = exception.Message;

    HResult = exception.HResult;
    HelpLink = exception.HelpLink;
    Source = exception.Source;
    StackTrace = exception.StackTrace;
    TargetSite = exception.TargetSite?.ToString();

    foreach (object key in exception.Data.Keys)
    {
      try
      {
        object? value = exception.Data[key];
        if (value != null)
        {
          string serializedKey = JsonSerializer.Serialize(key, key.GetType(), serializerOptions).Trim('"');
          string serializedValue = JsonSerializer.Serialize(value, value.GetType(), serializerOptions).Trim('"');
          Data[serializedKey] = serializedValue;
        }
      }
      catch (Exception)
      {
      }
    }
  }

  private LogException()
  {
  }
}
