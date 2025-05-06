using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Templates
{
  public static readonly TableId Table = new(Schemas.Messaging, nameof(KrakenarContext.Templates), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Template.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Template.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(Template.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Template.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Template.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Template.Version), Table);

  public static readonly ColumnId ContentText = new(nameof(Template.ContentText), Table);
  public static readonly ColumnId ContentType = new(nameof(Template.ContentType), Table);
  public static readonly ColumnId Description = new(nameof(Template.Description), Table);
  public static readonly ColumnId DisplayName = new(nameof(Template.DisplayName), Table);
  public static readonly ColumnId Id = new(nameof(Template.Id), Table);
  public static readonly ColumnId RealmId = new(nameof(Template.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(Template.RealmUid), Table);
  public static readonly ColumnId Subject = new(nameof(Template.Subject), Table);
  public static readonly ColumnId TemplateId = new(nameof(Template.TemplateId), Table);
  public static readonly ColumnId UniqueName = new(nameof(Template.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(Template.UniqueNameNormalized), Table);
}
