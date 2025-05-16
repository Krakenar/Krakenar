using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class FieldDefinitions
{
  public static readonly TableId Table = new(Schemas.Content, nameof(KrakenarContext.FieldDefinitions), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(FieldDefinition.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(FieldDefinition.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(FieldDefinition.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(FieldDefinition.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(FieldDefinition.Version), Table);

  public static readonly ColumnId ContentTypeId = new(nameof(FieldDefinition.ContentTypeId), Table);
  public static readonly ColumnId ContentTypeUid = new(nameof(FieldDefinition.ContentTypeUid), Table);
  public static readonly ColumnId Description = new(nameof(FieldDefinition.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(FieldDefinition.DisplayName), Table);
  public static readonly ColumnId FieldDefinitionId = new(nameof(FieldDefinition.FieldDefinitionId), Table);
  public static readonly ColumnId FieldTypeId = new(nameof(FieldDefinition.FieldTypeId), Table);
  public static readonly ColumnId FieldTypeUid = new(nameof(FieldDefinition.FieldTypeUid), Table);
  public static readonly ColumnId Id = new(nameof(FieldDefinition.Id), Table);
  public static readonly ColumnId IsIndexed = new(nameof(FieldDefinition.IsIndexed), Table);
  public static readonly ColumnId IsInvariant = new(nameof(FieldDefinition.IsInvariant), Table);
  public static readonly ColumnId IsRequired = new(nameof(FieldDefinition.IsRequired), Table);
  public static readonly ColumnId IsUnique = new(nameof(FieldDefinition.IsUnique), Table);
  public static readonly ColumnId Order = new(nameof(FieldDefinition.Order), Table);
  public static readonly ColumnId Placeholder = new(nameof(FieldDefinition.Placeholder), Table);
  public static readonly ColumnId UniqueName = new(nameof(FieldDefinition.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(FieldDefinition.UniqueNameNormalized), Table);
}
