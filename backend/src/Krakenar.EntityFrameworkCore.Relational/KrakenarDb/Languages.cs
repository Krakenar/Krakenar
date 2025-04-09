using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Languages
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.Languages), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Language.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Language.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(Language.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Language.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Language.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Language.Version), Table);

  public static readonly ColumnId Code = new(nameof(Language.Code), Table);
  public static readonly ColumnId CodeNormalized = new(nameof(Language.CodeNormalized), Table);
  public static readonly ColumnId DisplayName = new(nameof(Language.DisplayName), Table);
  public static readonly ColumnId EnglishName = new(nameof(Language.EnglishName), Table);
  public static readonly ColumnId Id = new(nameof(Language.Id), Table);
  public static readonly ColumnId IsDefault = new(nameof(Language.IsDefault), Table);
  public static readonly ColumnId LanguageId = new(nameof(Language.LanguageId), Table);
  public static readonly ColumnId NativeName = new(nameof(Language.NativeName), Table);
  public static readonly ColumnId RealmId = new(nameof(Language.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(Language.RealmUid), Table);
}
