using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class LogExceptions
{
  public static readonly TableId Table = new(Schemas.Logging, nameof(KrakenarContext.LogExceptions), alias: null);

  public static readonly ColumnId Data = new(nameof(LogException.Data), Table);
  public static readonly ColumnId HelpLink = new(nameof(LogException.HelpLink), Table);
  public static readonly ColumnId HResult = new(nameof(LogException.HResult), Table);
  public static readonly ColumnId Id = new(nameof(LogException.Id), Table);
  public static readonly ColumnId LogExceptionId = new(nameof(LogException.LogExceptionId), Table);
  public static readonly ColumnId LogId = new(nameof(LogException.LogId), Table);
  public static readonly ColumnId LogUid = new(nameof(LogException.LogUid), Table);
  public static readonly ColumnId Message = new(nameof(LogException.Message), Table);
  public static readonly ColumnId Source = new(nameof(LogException.Source), Table);
  public static readonly ColumnId StackTrace = new(nameof(LogException.StackTrace), Table);
  public static readonly ColumnId TargetSite = new(nameof(LogException.TargetSite), Table);
  public static readonly ColumnId Type = new(nameof(LogException.Type), Table);
}
