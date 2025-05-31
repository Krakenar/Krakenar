using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Logs
{
  public static readonly TableId Table = new(Schemas.Logging, nameof(KrakenarContext.Logs), alias: null);

  public static readonly ColumnId ActivityData = new(nameof(Log.ActivityData), Table);
  public static readonly ColumnId ActivityType = new(nameof(Log.ActivityType), Table);
  public static readonly ColumnId ActorId = new(nameof(Log.ActorId), Table);
  public static readonly ColumnId AdditionalInformation = new(nameof(Log.AdditionalInformation), Table);
  public static readonly ColumnId ApiKeyId = new(nameof(Log.ApiKeyId), Table);
  public static readonly ColumnId CorrelationId = new(nameof(Log.CorrelationId), Table);
  public static readonly ColumnId Destination = new(nameof(Log.Destination), Table);
  public static readonly ColumnId Duration = new(nameof(Log.Duration), Table);
  public static readonly ColumnId EndedOn = new(nameof(Log.EndedOn), Table);
  public static readonly ColumnId HasErrors = new(nameof(Log.HasErrors), Table);
  public static readonly ColumnId Id = new(nameof(Log.Id), Table);
  public static readonly ColumnId IsCompleted = new(nameof(Log.IsCompleted), Table);
  public static readonly ColumnId Level = new(nameof(Log.Level), Table);
  public static readonly ColumnId LogId = new(nameof(Log.LogId), Table);
  public static readonly ColumnId Method = new(nameof(Log.Method), Table);
  public static readonly ColumnId OperationName = new(nameof(Log.OperationName), Table);
  public static readonly ColumnId OperationType = new(nameof(Log.OperationType), Table);
  public static readonly ColumnId RealmId = new(nameof(Log.RealmId), Table);
  public static readonly ColumnId SessionId = new(nameof(Log.SessionId), Table);
  public static readonly ColumnId Source = new(nameof(Log.Source), Table);
  public static readonly ColumnId StartedOn = new(nameof(Log.StartedOn), Table);
  public static readonly ColumnId StatusCode = new(nameof(Log.StatusCode), Table);
  public static readonly ColumnId UserId = new(nameof(Log.UserId), Table);
}
