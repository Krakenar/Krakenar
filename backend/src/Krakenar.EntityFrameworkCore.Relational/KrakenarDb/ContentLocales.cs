using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class ContentLocales
{
  public static readonly TableId Table = new(Schemas.Content, nameof(KrakenarContext.ContentLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(ContentLocale.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ContentLocale.CreatedOn), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ContentLocale.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ContentLocale.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ContentLocale.Version), Table);

  public static readonly ColumnId ContentId = new(nameof(ContentLocale.ContentId), Table);
  public static readonly ColumnId ContentLocaleId = new(nameof(ContentLocale.ContentLocaleId), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(ContentLocale.ContentTypeId), Table);
  public static readonly ColumnId ContentTypeUid = new(nameof(ContentLocale.ContentTypeUid), Table);
  public static readonly ColumnId ContentUid = new(nameof(ContentLocale.ContentUid), Table);
  public static readonly ColumnId Description = new(nameof(ContentLocale.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(ContentLocale.DisplayName), Table);
  public static readonly ColumnId LanguageId = new(nameof(ContentLocale.LanguageId), Table);
  public static readonly ColumnId LanguageUid = new(nameof(ContentLocale.LanguageUid), Table);
  public static readonly ColumnId UniqueName = new(nameof(ContentLocale.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentLocale.UniqueNameNormalized), Table);
}
