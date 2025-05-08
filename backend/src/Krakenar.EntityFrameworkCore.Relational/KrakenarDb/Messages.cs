using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Messages
{
  public static readonly TableId Table = new(Schemas.Messaging, nameof(KrakenarContext.Messages), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Message.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Message.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(Message.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Message.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Message.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Message.Version), Table);

  public static readonly ColumnId BodyText = new(nameof(Message.BodyText), Table);
  public static readonly ColumnId BodyType = new(nameof(Message.BodyType), Table);
  public static readonly ColumnId Id = new(nameof(Message.Id), Table);
  public static readonly ColumnId IgnoreUserLocale = new(nameof(Message.IgnoreUserLocale), Table);
  public static readonly ColumnId IsDemo = new(nameof(Message.IsDemo), Table);
  public static readonly ColumnId Locale = new(nameof(Message.Locale), Table);
  public static readonly ColumnId MessageId = new(nameof(Message.MessageId), Table);
  public static readonly ColumnId RealmId = new(nameof(Message.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(Message.RealmUid), Table);
  public static readonly ColumnId RecipientCount = new(nameof(Message.RecipientCount), Table);
  public static readonly ColumnId Results = new(nameof(Message.Results), Table);
  public static readonly ColumnId SenderDisplayName = new(nameof(Message.SenderDisplayName), Table);
  public static readonly ColumnId SenderEmailAddress = new(nameof(Message.SenderEmailAddress), Table);
  public static readonly ColumnId SenderId = new(nameof(Message.SenderId), Table);
  public static readonly ColumnId SenderIsDefault = new(nameof(Message.SenderIsDefault), Table);
  public static readonly ColumnId SenderPhoneCountryCode = new(nameof(Message.SenderPhoneCountryCode), Table);
  public static readonly ColumnId SenderPhoneE164Formatted = new(nameof(Message.SenderPhoneE164Formatted), Table);
  public static readonly ColumnId SenderPhoneExtension = new(nameof(Message.SenderPhoneExtension), Table);
  public static readonly ColumnId SenderPhoneNumber = new(nameof(Message.SenderPhoneNumber), Table);
  public static readonly ColumnId SenderProvider = new(nameof(Message.SenderProvider), Table);
  public static readonly ColumnId SenderUid = new(nameof(Message.SenderUid), Table);
  public static readonly ColumnId Status = new(nameof(Message.Status), Table);
  public static readonly ColumnId Subject = new(nameof(Message.Subject), Table);
  public static readonly ColumnId TemplateDisplayName = new(nameof(Message.TemplateDisplayName), Table);
  public static readonly ColumnId TemplateId = new(nameof(Message.TemplateId), Table);
  public static readonly ColumnId TemplateUid = new(nameof(Message.TemplateUid), Table);
  public static readonly ColumnId TemplateUniqueName = new(nameof(Message.TemplateUniqueName), Table);
  public static readonly ColumnId Variables = new(nameof(Message.Variables), Table);
}
