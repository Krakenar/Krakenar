using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class LogEvents
{
  public static readonly TableId Table = new(Schemas.Logging, nameof(KrakenarContext.LogEvents), alias: null);

  public static readonly ColumnId EventId = new(nameof(LogEvent.EventId), Table);
  public static readonly ColumnId LogEventId = new(nameof(LogEvent.LogEventId), Table);
  public static readonly ColumnId LogId = new(nameof(LogEvent.LogId), Table);
  public static readonly ColumnId LogUid = new(nameof(LogEvent.LogUid), Table);
}
