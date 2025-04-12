using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class UserRoles
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.UserRoles), alias: null);

  public static readonly ColumnId UserId = new(nameof(UserRole.UserId), Table);
  public static readonly ColumnId RoleId = new(nameof(UserRole.RoleId), Table);
}
