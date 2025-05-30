namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public abstract class Identifier : ISegregatedEntity
{
  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public string Key { get; private set; } = string.Empty;
  public string Value { get; protected set; } = string.Empty;

  protected Identifier()
  {
  }

  protected Identifier(Realm? realm, string key)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
    RealmUid = realm?.Id;

    Key = key;
  }
}
