using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Realms
{
  public static readonly TableId Table = new(Schemas.Krakenar, nameof(KrakenarContext.Realms), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Realm.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Realm.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(Realm.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Realm.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Realm.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Realm.Version), Table);

  public static readonly ColumnId AllowedUniqueNameCharacters = new(nameof(Realm.AllowedUniqueNameCharacters), Table);
  public static readonly ColumnId CustomAttributes = new(nameof(Realm.CustomAttributes), Table);
  public static readonly ColumnId Description = new(nameof(Realm.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(Realm.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(Realm.Id), Table);
  public static readonly ColumnId PasswordHashingStrategy = new(nameof(Realm.PasswordHashingStrategy), Table);
  public static readonly ColumnId PasswordsRequireDigit = new(nameof(Realm.PasswordsRequireDigit), Table);
  public static readonly ColumnId PasswordsRequireLowercase = new(nameof(Realm.PasswordsRequireLowercase), Table);
  public static readonly ColumnId PasswordsRequireNonAlphanumeric = new(nameof(Realm.PasswordsRequireNonAlphanumeric), Table);
  public static readonly ColumnId PasswordsRequireUppercase = new(nameof(Realm.PasswordsRequireUppercase), Table);
  public static readonly ColumnId RealmId = new(nameof(Realm.RealmId), Table);
  public static readonly ColumnId RequireConfirmedAccount = new(nameof(Realm.RequireConfirmedAccount), Table);
  public static readonly ColumnId RequiredPasswordLength = new(nameof(Realm.RequiredPasswordLength), Table);
  public static readonly ColumnId RequiredPasswordUniqueChars = new(nameof(Realm.RequiredPasswordUniqueChars), Table);
  public static readonly ColumnId RequireUniqueEmail = new(nameof(Realm.RequireUniqueEmail), Table);
  public static readonly ColumnId Secret = new(nameof(Realm.Secret), Table);
  public static readonly ColumnId SecretChangedBy = new(nameof(Realm.SecretChangedBy), Table);
  public static readonly ColumnId SecretChangedOn = new(nameof(Realm.SecretChangedOn), Table);
  public static readonly ColumnId UniqueSlug = new(nameof(Realm.UniqueSlug), Table);
  public static readonly ColumnId UniqueSlugNormalized = new(nameof(Realm.UniqueSlugNormalized), Table);
  public static readonly ColumnId Url = new(nameof(Realm.Url), Table);
}
