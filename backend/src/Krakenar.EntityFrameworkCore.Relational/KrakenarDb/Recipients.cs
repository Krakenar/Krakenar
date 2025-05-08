using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Recipients
{
  public static readonly TableId Table = new(Schemas.Messaging, nameof(KrakenarContext.Recipients), alias: null);

  public static readonly ColumnId DisplayName = new(nameof(Recipient.DisplayName), Table);
  public static readonly ColumnId EmailAddress = new(nameof(Recipient.EmailAddress), Table);
  public static readonly ColumnId Id = new(nameof(Recipient.Id), Table);
  public static readonly ColumnId MessageId = new(nameof(Recipient.MessageId), Table);
  public static readonly ColumnId PhoneCountryCode = new(nameof(Recipient.PhoneCountryCode), Table);
  public static readonly ColumnId PhoneE164Formatted = new(nameof(Recipient.PhoneE164Formatted), Table);
  public static readonly ColumnId PhoneExtension = new(nameof(Recipient.PhoneExtension), Table);
  public static readonly ColumnId PhoneNumber = new(nameof(Recipient.PhoneNumber), Table);
  public static readonly ColumnId RecipientId = new(nameof(Recipient.RecipientId), Table);
  public static readonly ColumnId Type = new(nameof(Recipient.Type), Table);
  public static readonly ColumnId UserFullName = new(nameof(Recipient.UserFullName), Table);
  public static readonly ColumnId UserId = new(nameof(Recipient.UserId), Table);
  public static readonly ColumnId UserPicture = new(nameof(Recipient.UserPicture), Table);
  public static readonly ColumnId UserUid = new(nameof(Recipient.UserUid), Table);
  public static readonly ColumnId UserUniqueName = new(nameof(Recipient.UserUniqueName), Table);
}
