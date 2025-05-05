using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class OneTimePasswords
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.OneTimePasswords), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(OneTimePassword.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(OneTimePassword.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(OneTimePassword.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(OneTimePassword.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(OneTimePassword.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(OneTimePassword.Version), Table);

  public static readonly ColumnId AttemptCount = new(nameof(OneTimePassword.AttemptCount), Table);
  public static readonly ColumnId CustomAttributes = new(nameof(OneTimePassword.CustomAttributes), Table);
  public static readonly ColumnId ExpiresOn = new(nameof(OneTimePassword.ExpiresOn), Table);
  public static readonly ColumnId Id = new(nameof(OneTimePassword.Id), Table);
  public static readonly ColumnId MaximumAttempts = new(nameof(OneTimePassword.MaximumAttempts), Table);
  public static readonly ColumnId OneTimePasswordId = new(nameof(OneTimePassword.OneTimePasswordId), Table);
  public static readonly ColumnId PasswordHash = new(nameof(OneTimePassword.PasswordHash), Table);
  public static readonly ColumnId RealmId = new(nameof(OneTimePassword.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(OneTimePassword.RealmUid), Table);
  public static readonly ColumnId UserId = new(nameof(OneTimePassword.UserId), Table);
  public static readonly ColumnId ValidationSucceededOn = new(nameof(OneTimePassword.ValidationSucceededOn), Table);
}
