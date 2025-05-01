using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class DictionaryEntries
{
  public static readonly TableId Table = new(Schemas.Localization, nameof(KrakenarContext.DictionaryEntries), alias: null);

  public static readonly ColumnId DictionaryEntryId = new(nameof(DictionaryEntry.DictionaryEntryId), Table);
  public static readonly ColumnId DictionaryId = new(nameof(DictionaryEntry.DictionaryId), Table);
  public static readonly ColumnId Key = new(nameof(DictionaryEntry.Key), Table);
  public static readonly ColumnId Value = new(nameof(DictionaryEntry.Value), Table);
  public static readonly ColumnId ValueShortened = new(nameof(DictionaryEntry.ValueShortened), Table);
}
