using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class FieldTypes
{
  public static readonly TableId Table = new(Schemas.Content, nameof(KrakenarContext.FieldTypes), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(FieldType.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(FieldType.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(FieldType.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(FieldType.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(FieldType.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(FieldType.Version), Table);

  public static readonly ColumnId DataType = new(nameof(FieldType.DataType), Table);
  public static readonly ColumnId Description = new(nameof(FieldType.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(FieldType.DisplayName), Table);
  public static readonly ColumnId FieldTypeId = new(nameof(FieldType.FieldTypeId), Table);
  public static readonly ColumnId Id = new(nameof(FieldType.Id), Table);
  public static readonly ColumnId RealmId = new(nameof(FieldType.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(FieldType.RealmUid), Table);
  public static readonly ColumnId Settings = new(nameof(FieldType.Settings), Table);
  public static readonly ColumnId UniqueName = new(nameof(FieldType.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(FieldType.UniqueNameNormalized), Table);
}
