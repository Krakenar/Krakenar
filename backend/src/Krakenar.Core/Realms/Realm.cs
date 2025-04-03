using Logitar.EventSourcing;

namespace Krakenar.Core.Realms;

public class Realm : AggregateRoot
{
  public const string EntityType = "Realm";

  public new RealmId Id => new(base.Id);
}
