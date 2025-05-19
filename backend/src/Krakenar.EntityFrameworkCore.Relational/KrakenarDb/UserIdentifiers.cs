using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class UserIdentifiers
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.UserIdentifiers), alias: null);

  public static readonly ColumnId Key = new(nameof(UserIdentifier.Key), Table);
  public static readonly ColumnId RealmId = new(nameof(UserIdentifier.RealmId), Table);
  public static readonly ColumnId UserId = new(nameof(UserIdentifier.UserId), Table);
  public static readonly ColumnId UserIdentifierId = new(nameof(UserIdentifier.UserIdentifierId), Table);
  public static readonly ColumnId UserUid = new(nameof(UserIdentifier.UserUid), Table);
  public static readonly ColumnId Value = new(nameof(UserIdentifier.Value), Table);
}
