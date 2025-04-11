using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar;

internal class TestApplicationContext : IApplicationContext
{
  public ActorId? ActorId { get; set; }
  public RealmDto? Realm { get; set; }
  public RealmId? RealmId => Realm is null ? null : new(Realm.Id);

  public IUniqueNameSettings UniqueNameSettings => throw new NotImplementedException();
  public IPasswordSettings PasswordSettings => throw new NotImplementedException();
  public bool RequireUniqueEmail => throw new NotImplementedException();
  public bool RequireConfirmedAccount => throw new NotImplementedException();
}
