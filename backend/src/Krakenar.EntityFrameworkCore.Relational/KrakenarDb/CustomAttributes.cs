using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class CustomAttributes
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.CustomAttributes), alias: null);

  public static readonly ColumnId CustomAttributeId = new(nameof(CustomAttribute.CustomAttributeId), Table);
  public static readonly ColumnId Entity = new(nameof(CustomAttribute.Entity), Table);
  public static readonly ColumnId Key = new(nameof(CustomAttribute.Key), Table);
  public static readonly ColumnId Value = new(nameof(CustomAttribute.Value), Table);
  public static readonly ColumnId ValueShortened = new(nameof(CustomAttribute.ValueShortened), Table);
}
