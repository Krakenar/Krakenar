using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class FieldIndex
{
  public static readonly TableId Table = new(Schemas.Content, nameof(KrakenarContext.FieldIndex), alias: null);

  public static readonly ColumnId ContentId = new(nameof(Entities.FieldIndex.ContentId), Table);
  public static readonly ColumnId ContentLocaleId = new(nameof(Entities.FieldIndex.ContentLocaleId), Table);
  public static readonly ColumnId ContentLocaleName = new(nameof(Entities.FieldIndex.ContentLocaleName), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(Entities.FieldIndex.ContentTypeId), Table);
  public static readonly ColumnId ContentTypeName = new(nameof(Entities.FieldIndex.ContentTypeName), Table);
  public static readonly ColumnId ContentTypeUid = new(nameof(Entities.FieldIndex.ContentTypeUid), Table);
  public static readonly ColumnId ContentUid = new(nameof(Entities.FieldIndex.ContentUid), Table);
  public static readonly ColumnId FieldDefinitionId = new(nameof(Entities.FieldIndex.FieldDefinitionId), Table);
  public static readonly ColumnId FieldDefinitionName = new(nameof(Entities.FieldIndex.FieldTypeName), Table);
  public static readonly ColumnId FieldDefinitionUid = new(nameof(Entities.FieldIndex.FieldDefinitionUid), Table);
  public static readonly ColumnId FieldIndexId = new(nameof(Entities.FieldIndex.FieldIndexId), Table);
  public static readonly ColumnId FieldTypeId = new(nameof(Entities.FieldIndex.FieldTypeId), Table);
  public static readonly ColumnId FieldTypeName = new(nameof(Entities.FieldIndex.FieldTypeName), Table);
  public static readonly ColumnId FieldTypeUid = new(nameof(Entities.FieldIndex.FieldTypeUid), Table);
  public static readonly ColumnId LanguageCode = new(nameof(Entities.FieldIndex.LanguageCode), Table);
  public static readonly ColumnId LanguageId = new(nameof(Entities.FieldIndex.LanguageId), Table);
  public static readonly ColumnId LanguageIsDefault = new(nameof(Entities.FieldIndex.LanguageIsDefault), Table);
  public static readonly ColumnId LanguageUid = new(nameof(Entities.FieldIndex.LanguageUid), Table);
  public static readonly ColumnId Status = new(nameof(Entities.FieldIndex.Status), Table);
  public static readonly ColumnId Version = new(nameof(Entities.FieldIndex.Version), Table);

  public static readonly ColumnId Boolean = new(nameof(Entities.FieldIndex.Boolean), Table);
  public static readonly ColumnId DateTime = new(nameof(Entities.FieldIndex.DateTime), Table);
  public static readonly ColumnId Number = new(nameof(Entities.FieldIndex.Number), Table);
  public static readonly ColumnId RelatedContent = new(nameof(Entities.FieldIndex.RelatedContent), Table);
  public static readonly ColumnId RichText = new(nameof(Entities.FieldIndex.RichText), Table);
  public static readonly ColumnId Select = new(nameof(Entities.FieldIndex.Select), Table);
  public static readonly ColumnId String = new(nameof(Entities.FieldIndex.String), Table);
  public static readonly ColumnId Tags = new(nameof(Entities.FieldIndex.Tags), Table);
}
