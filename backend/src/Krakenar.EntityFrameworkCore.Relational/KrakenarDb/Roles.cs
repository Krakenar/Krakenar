using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Roles
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.Roles), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Role.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Role.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(Role.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Role.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Role.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Role.Version), Table);

  public static readonly ColumnId CustomAttributes = new(nameof(Role.CustomAttributes), Table);
  public static readonly ColumnId Description = new(nameof(Role.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(Role.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(Role.Id), Table);
  public static readonly ColumnId RealmId = new(nameof(Role.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(Role.RealmUid), Table);
  public static readonly ColumnId RoleId = new(nameof(Role.RoleId), Table);
  public static readonly ColumnId UniqueName = new(nameof(Role.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(Role.UniqueNameNormalized), Table);
}
