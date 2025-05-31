using Logitar;
using Logitar.EventSourcing;
using Microsoft.Extensions.Logging;
using CoreLog = Krakenar.Core.Logging.Log;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Log
{
  public long LogId { get; private set; }
  public Guid Id { get; private set; }

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

  public List<LogEvent> Events { get; private set; } = [];
  public List<LogException> Exceptions { get; private set; } = [];

  public Log(CoreLog log, JsonSerializerOptions? serializerOptions = null)
  {
    Id = log.Id;

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
        Events.Add(new LogEvent(this, identifiable));
      }
    }
    foreach (Exception exception in log.Exceptions)
    {
      Exceptions.Add(new LogException(this, exception, serializerOptions));
    }
  }

  private Log()
  {
  }
}
