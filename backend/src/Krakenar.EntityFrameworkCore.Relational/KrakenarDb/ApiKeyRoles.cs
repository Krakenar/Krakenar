using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class ApiKeyRoles
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.ApiKeyRoles), alias: null);

  public static readonly ColumnId ApiKeyId = new(nameof(ApiKeyRole.ApiKeyId), Table);
  public static readonly ColumnId RoleId = new(nameof(ApiKeyRole.RoleId), Table);
}
