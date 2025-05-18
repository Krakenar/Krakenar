using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class PublishedContents
{
  public static readonly TableId Table = new(Schemas.Content, nameof(KrakenarContext.PublishedContents), alias: null);

  public static readonly ColumnId ContentId = new(nameof(PublishedContent.ContentId), Table);
  public static readonly ColumnId ContentLocaleId = new(nameof(PublishedContent.ContentLocaleId), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(PublishedContent.ContentTypeId), Table);
  public static readonly ColumnId ContentTypeName = new(nameof(PublishedContent.ContentTypeName), Table);
  public static readonly ColumnId ContentTypeUid = new(nameof(PublishedContent.ContentTypeUid), Table);
  public static readonly ColumnId ContentUid = new(nameof(PublishedContent.ContentUid), Table);
  public static readonly ColumnId Description = new(nameof(PublishedContent.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(PublishedContent.DisplayName), Table);
  public static readonly ColumnId FieldValues = new(nameof(PublishedContent.FieldValues), Table);
  public static readonly ColumnId LanguageCode = new(nameof(PublishedContent.LanguageCode), Table);
  public static readonly ColumnId LanguageId = new(nameof(PublishedContent.LanguageId), Table);
  public static readonly ColumnId LanguageIsDefault = new(nameof(PublishedContent.LanguageIsDefault), Table);
  public static readonly ColumnId LanguageUid = new(nameof(PublishedContent.LanguageUid), Table);
  public static readonly ColumnId PublishedBy = new(nameof(PublishedContent.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(PublishedContent.PublishedOn), Table);
  public static readonly ColumnId UniqueName = new(nameof(PublishedContent.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(PublishedContent.UniqueNameNormalized), Table);
  public static readonly ColumnId Version = new(nameof(PublishedContent.Version), Table);
}
