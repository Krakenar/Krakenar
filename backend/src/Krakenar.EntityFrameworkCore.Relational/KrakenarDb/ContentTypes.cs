using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class ContentTypes
{
  public static readonly TableId Table = new(Schemas.Content, nameof(KrakenarContext.ContentTypes), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(ContentType.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ContentType.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(ContentType.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ContentType.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ContentType.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ContentType.Version), Table);

  public static readonly ColumnId ContentTypeId = new(nameof(ContentType.ContentTypeId), Table);
  public static readonly ColumnId Description = new(nameof(ContentType.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(ContentType.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(ContentType.Id), Table);
  public static readonly ColumnId IsInvariant = new(nameof(ContentType.IsInvariant), Table);
  public static readonly ColumnId RealmId = new(nameof(ContentType.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(ContentType.RealmUid), Table);
  public static readonly ColumnId UniqueName = new(nameof(ContentType.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(ContentType.UniqueNameNormalized), Table);
}
