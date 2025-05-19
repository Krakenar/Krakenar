using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Dictionaries
{
  public static readonly TableId Table = new(Schemas.Localization, nameof(KrakenarContext.Dictionaries), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Dictionary.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Dictionary.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(Dictionary.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Dictionary.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Dictionary.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Dictionary.Version), Table);

  public static readonly ColumnId DictionaryId = new(nameof(Dictionary.DictionaryId), Table);
  public static readonly ColumnId EntryCount = new(nameof(Dictionary.EntryCount), Table);
  public static readonly ColumnId Id = new(nameof(Dictionary.Id), Table);
  public static readonly ColumnId LanguageId = new(nameof(Dictionary.LanguageId), Table);
  public static readonly ColumnId LanguageUid = new(nameof(Dictionary.LanguageUid), Table);
  public static readonly ColumnId RealmId = new(nameof(Dictionary.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(Dictionary.RealmUid), Table);
}
