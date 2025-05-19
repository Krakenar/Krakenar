using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Sessions
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.Sessions), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Session.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Session.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(Session.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Session.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Session.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Session.Version), Table);

  public static readonly ColumnId CustomAttributes = new(nameof(Session.CustomAttributes), Table);
  public static readonly ColumnId Id = new(nameof(Session.Id), Table);
  public static readonly ColumnId IsActive = new(nameof(Session.IsActive), Table);
  public static readonly ColumnId IsPersistent = new(nameof(Session.IsPersistent), Table);
  public static readonly ColumnId RealmId = new(nameof(Session.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(Session.RealmUid), Table);
  public static readonly ColumnId SecretHash = new(nameof(Session.SecretHash), Table);
  public static readonly ColumnId SessionId = new(nameof(Session.SessionId), Table);
  public static readonly ColumnId SignedOutBy = new(nameof(Session.SignedOutBy), Table);
  public static readonly ColumnId SignedOutOn = new(nameof(Session.SignedOutOn), Table);
  public static readonly ColumnId UserId = new(nameof(Session.UserId), Table);
  public static readonly ColumnId UserUid = new(nameof(Session.UserUid), Table);
}
