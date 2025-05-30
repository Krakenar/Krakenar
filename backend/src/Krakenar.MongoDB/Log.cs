using Logitar;
using Logitar.EventSourcing;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CoreLog = Krakenar.Core.Logging.Log;

namespace Krakenar.MongoDB;

public sealed class Log
{
  [BsonId]
  public ObjectId LogId { get; private set; }

  [BsonRepresentation(BsonType.String)]
  public Guid UniqueId { get; private set; }

  public string? CorrelationId { get; private set; }
  public string? Method { get; private set; }
  public string? Destination { get; private set; }
  public string? Source { get; private set; }
  public string? AdditionalInformation { get; private set; }

  public string? OperationType { get; private set; }
  public string? OperationName { get; private set; }

  public string? ActivityType { get; private set; }
  public string? ActivityData { get; private set; }

  public int? StatusCode { get; private set; }
  public bool IsCompleted { get; private set; }

  [JsonConverter(typeof(JsonStringEnumConverter))]
  [BsonRepresentation(BsonType.String)]
  public LogLevel Level { get; private set; }
  public bool HasErrors { get; private set; }

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration { get; private set; }

  public string? RealmId { get; private set; }
  public string? ApiKeyId { get; private set; }
  public string? SessionId { get; private set; }
  public string? UserId { get; private set; }
  public string? ActorId { get; private set; }

  public List<string> EventIds { get; private set; } = [];

  public List<LogException> Exceptions { get; private set; } = [];

  public Log(CoreLog log, JsonSerializerOptions? serializerOptions = null)
  {
    UniqueId = log.Id;

    CorrelationId = log.CorrelationId;
    Method = log.Method;
    Destination = log.Destination;
    Source = log.Source;
    AdditionalInformation = log.AdditionalInformation;

    if (log.Operation is not null)
    {
      OperationType = log.Operation.Type;
      OperationName = log.Operation.Name;
    }

    if (log.Activity is not null)
    {
      Type activityType = log.Activity.GetType();
      ActivityType = activityType.GetNamespaceQualifiedName();
      ActivityData = JsonSerializer.Serialize(log.Activity, activityType, serializerOptions);
    }

    StatusCode = log.StatusCode;
    IsCompleted = log.IsCompleted;

    Level = log.Level;
    HasErrors = log.HasErrors;

    StartedOn = log.StartedOn.ToUniversalTime();
    EndedOn = log.EndedOn?.ToUniversalTime();
    Duration = log.Duration;

    RealmId = log.RealmId?.Value;
    ApiKeyId = log.ApiKeyId?.Value;
    SessionId = log.SessionId?.Value;
    UserId = log.UserId?.Value;
    ActorId = log.ActorId?.Value;

    foreach (IEvent @event in log.Events)
    {
      if (@event is IIdentifiableEvent identifiable)
      {
        EventIds.Add(identifiable.Id.Value);
      }
    }
    foreach (Exception exception in log.Exceptions)
    {
      Exceptions.Add(new LogException(exception, serializerOptions));
    }
  }

  private Log()
  {
  }
}
