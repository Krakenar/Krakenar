using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class ApiKeys
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.ApiKeys), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(ApiKey.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ApiKey.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(ApiKey.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ApiKey.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ApiKey.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ApiKey.Version), Table);

  public static readonly ColumnId ApiKeyId = new(nameof(ApiKey.ApiKeyId), Table);
  public static readonly ColumnId AuthenticatedOn = new(nameof(ApiKey.AuthenticatedOn), Table);
  public static readonly ColumnId CustomAttributes = new(nameof(ApiKey.CustomAttributes), Table);
  public static readonly ColumnId Description = new(nameof(ApiKey.Description), Table);
  public static readonly ColumnId ExpiresOn = new(nameof(ApiKey.ExpiresOn), Table);
  public static readonly ColumnId Id = new(nameof(ApiKey.Id), Table);
  public static readonly ColumnId Name = new(nameof(ApiKey.Name), Table);
  public static readonly ColumnId RealmId = new(nameof(ApiKey.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(ApiKey.RealmUid), Table);
  public static readonly ColumnId SecretHash = new(nameof(ApiKey.SecretHash), Table);
}
