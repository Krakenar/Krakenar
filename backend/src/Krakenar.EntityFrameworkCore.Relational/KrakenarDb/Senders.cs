using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Senders
{
  public static readonly TableId Table = new(Schemas.Messaging, nameof(KrakenarContext.Senders), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Sender.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Sender.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(Sender.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Sender.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Sender.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Sender.Version), Table);

  public static readonly ColumnId Description = new(nameof(Sender.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(Sender.DisplayName), Table);
  public static readonly ColumnId EmailAddress = new(nameof(Sender.EmailAddress), Table);
  public static readonly ColumnId Id = new(nameof(Sender.Id), Table);
  public static readonly ColumnId IsDefault = new(nameof(Sender.IsDefault), Table);
  public static readonly ColumnId Kind = new(nameof(Sender.Kind), Table);
  public static readonly ColumnId PhoneCountryCode = new(nameof(Sender.PhoneCountryCode), Table);
  public static readonly ColumnId PhoneE164Formatted = new(nameof(Sender.PhoneE164Formatted), Table);
  public static readonly ColumnId PhoneNumber = new(nameof(Sender.PhoneNumber), Table);
  public static readonly ColumnId Provider = new(nameof(Sender.Provider), Table);
  public static readonly ColumnId RealmId = new(nameof(Sender.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(Sender.RealmUid), Table);
  public static readonly ColumnId SenderId = new(nameof(Sender.SenderId), Table);
  public static readonly ColumnId Settings = new(nameof(Sender.Settings), Table);
}
