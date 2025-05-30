using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class LogEvent
{
  public long LogEventId { get; private set; }

  public Log? Log { get; private set; }
  public long LogId { get; private set; }
  public Guid LogUid { get; private set; }

  public string EventId { get; private set; } = string.Empty;

  public LogEvent(Log log, IIdentifiableEvent @event)
  {
    Log = log;
    LogId = log.LogId;
    LogUid = log.Id;

    EventId = @event.Id.Value;
  }

  private LogEvent()
  {
  }
}
