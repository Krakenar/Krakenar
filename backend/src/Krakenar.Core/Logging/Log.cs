using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;
using Microsoft.Extensions.Logging;

namespace Krakenar.Core.Logging;

public class Log
{
  public Guid Id { get; private set; }

  public string? CorrelationId { get; private set; }
  public string? Method { get; private set; }
  public string? Destination { get; private set; }
  public string? Source { get; private set; }
  public string? AdditionalInformation { get; private set; }

  public Operation? Operation { get; set; }
  public IActivity? Activity { get; set; }

  public int? StatusCode { get; private set; }
  public bool IsCompleted => StatusCode.HasValue;
  public void Close(int statusCode, DateTime? endedOn = null)
  {
    StatusCode = statusCode;

    EndedOn = endedOn ?? DateTime.Now;
  }

  public LogLevel Level => HasErrors ? LogLevel.Error : LogLevel.Information;
  public bool HasErrors => Exceptions.Count > 0;

  public DateTime StartedOn { get; private set; }
  public DateTime? EndedOn { get; private set; }
  public TimeSpan? Duration => EndedOn.HasValue ? EndedOn.Value - StartedOn : null;

  public Realm? Realm { get; set; }
  public ApiKey? ApiKey { get; set; }
  public Session? Session { get; set; }
  public User? User { get; set; }

  private readonly List<IEvent> _events = [];
  public IReadOnlyCollection<IEvent> Events => _events.AsReadOnly();
  public void Report(IEvent @event)
  {
    _events.Add(@event);
  }

  private readonly List<Exception> _exceptions = [];
  public IReadOnlyCollection<Exception> Exceptions => _exceptions.AsReadOnly();
  public void Report(Exception exception)
  {
    _exceptions.Add(exception);
  }

  public static Log Open(string? correlationId, string? method, string? destination, string? source, string? additionalInformation, Guid? id = null, DateTime? startedOn = null)
  {
    return new Log(correlationId, method, destination, source, additionalInformation, id, startedOn);
  }
  private Log(string? correlationId, string? method, string? destination, string? source, string? additionalInformation, Guid? id = null, DateTime? startedOn = null)
  {
    Id = id ?? Guid.NewGuid();

    CorrelationId = correlationId;
    Method = method;
    Destination = destination;
    Source = source;
    AdditionalInformation = additionalInformation;

    StartedOn = startedOn ?? DateTime.Now;
  }

  public override bool Equals(object? obj) => obj is Log log && log.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{GetType()} (Id={Id})";
}
