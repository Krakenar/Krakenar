namespace Krakenar.Contracts.Realms;

public record CreateOrReplaceRealmResult
{
  public Realm? Realm { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceRealmResult()
  {
  }

  public CreateOrReplaceRealmResult(Realm? realm, bool created)
  {
    Realm = realm;
    Created = created;
  }
}
