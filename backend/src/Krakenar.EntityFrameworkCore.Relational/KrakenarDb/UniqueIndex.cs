using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class UniqueIndex
{
  public static readonly TableId Table = new(Schemas.Content, nameof(KrakenarContext.UniqueIndex), alias: null);

  public static readonly ColumnId ContentId = new(nameof(Entities.UniqueIndex.ContentId), Table);
  public static readonly ColumnId ContentLocaleId = new(nameof(Entities.UniqueIndex.ContentLocaleId), Table);
  public static readonly ColumnId ContentLocaleName = new(nameof(Entities.UniqueIndex.ContentLocaleName), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(Entities.UniqueIndex.ContentTypeId), Table);
  public static readonly ColumnId ContentTypeName = new(nameof(Entities.UniqueIndex.ContentTypeName), Table);
  public static readonly ColumnId ContentTypeUid = new(nameof(Entities.UniqueIndex.ContentTypeUid), Table);
  public static readonly ColumnId ContentUid = new(nameof(Entities.UniqueIndex.ContentUid), Table);
  public static readonly ColumnId FieldDefinitionId = new(nameof(Entities.UniqueIndex.FieldDefinitionId), Table);
  public static readonly ColumnId FieldDefinitionName = new(nameof(Entities.UniqueIndex.FieldDefinitionName), Table);
  public static readonly ColumnId FieldDefinitionUid = new(nameof(Entities.UniqueIndex.FieldDefinitionUid), Table);
  public static readonly ColumnId FieldTypeId = new(nameof(Entities.UniqueIndex.FieldTypeId), Table);
  public static readonly ColumnId FieldTypeName = new(nameof(Entities.UniqueIndex.FieldTypeName), Table);
  public static readonly ColumnId FieldTypeUid = new(nameof(Entities.UniqueIndex.FieldTypeUid), Table);
  public static readonly ColumnId Key = new(nameof(Entities.UniqueIndex.Key), Table);
  public static readonly ColumnId LanguageCode = new(nameof(Entities.UniqueIndex.LanguageCode), Table);
  public static readonly ColumnId LanguageId = new(nameof(Entities.UniqueIndex.LanguageId), Table);
  public static readonly ColumnId LanguageIsDefault = new(nameof(Entities.UniqueIndex.LanguageIsDefault), Table);
  public static readonly ColumnId LanguageUid = new(nameof(Entities.UniqueIndex.LanguageUid), Table);
  public static readonly ColumnId Status = new(nameof(Entities.UniqueIndex.Status), Table);
  public static readonly ColumnId UniqueIndexId = new(nameof(Entities.UniqueIndex.UniqueIndexId), Table);
  public static readonly ColumnId Value = new(nameof(Entities.UniqueIndex.Value), Table);
  public static readonly ColumnId ValueNormalized = new(nameof(Entities.UniqueIndex.ValueNormalized), Table);
  public static readonly ColumnId Version = new(nameof(Entities.UniqueIndex.Version), Table);
}
