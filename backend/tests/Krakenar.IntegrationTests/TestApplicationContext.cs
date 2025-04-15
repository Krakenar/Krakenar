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

  public IUniqueNameSettings UniqueNameSettings => Realm?.UniqueNameSettings ?? throw new NotImplementedException();
  public IPasswordSettings PasswordSettings => Realm?.PasswordSettings ?? throw new NotImplementedException();
  public bool RequireUniqueEmail => Realm?.RequireUniqueEmail ?? false;
  public bool RequireConfirmedAccount => Realm?.RequireConfirmedAccount ?? false;
}
