using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Contents
{
  public static readonly TableId Table = new(Schemas.Content, nameof(KrakenarContext.Contents), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Content.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Content.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(Content.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Content.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Content.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Content.Version), Table);

  public static readonly ColumnId ContentId = new(nameof(Content.ContentId), Table);
  public static readonly ColumnId ContentTypeId = new(nameof(Content.ContentTypeId), Table);
  public static readonly ColumnId ContentTypeUid = new(nameof(Content.ContentTypeUid), Table);
  public static readonly ColumnId Id = new(nameof(Content.Id), Table);
  public static readonly ColumnId RealmId = new(nameof(Content.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(Content.RealmUid), Table);
}
