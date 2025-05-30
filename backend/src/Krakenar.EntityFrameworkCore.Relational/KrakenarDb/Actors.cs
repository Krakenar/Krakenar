using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Actors
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.Actors), alias: null);

  public static readonly ColumnId ActorId = new(nameof(Actor.Id), Table);
  public static readonly ColumnId DisplayName = new(nameof(Actor.DisplayName), Table);
  public static readonly ColumnId EmailAddress = new(nameof(Actor.EmailAddress), Table);
  public static readonly ColumnId Id = new(nameof(Actor.Id), Table);
  public static readonly ColumnId IsDeleted = new(nameof(Actor.IsDeleted), Table);
  public static readonly ColumnId Key = new(nameof(Actor.Key), Table);
  public static readonly ColumnId PictureUrl = new(nameof(Actor.PictureUrl), Table);
  public static readonly ColumnId RealmId = new(nameof(Actor.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(Actor.RealmUid), Table);
  public static readonly ColumnId Type = new(nameof(Actor.Type), Table);
}
