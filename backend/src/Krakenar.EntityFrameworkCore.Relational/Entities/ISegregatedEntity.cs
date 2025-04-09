namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public interface ISegregatedEntity
{
  Realm? Realm { get; }
  int? RealmId { get; }
  Guid? RealmUid { get; }
}
