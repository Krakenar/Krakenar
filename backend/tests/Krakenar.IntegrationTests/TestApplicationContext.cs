using Krakenar.Contracts.Settings;
using Krakenar.Core;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar;

internal class TestApplicationContext : IApplicationContext
{
  public ActorId? ActorId { get; set; }
  public ConfigurationDto Configuration { get; set; } = new();
  public RealmDto? Realm { get; set; }
  public RealmId? RealmId => Realm is null ? null : new(Realm.Id);

  public IUniqueNameSettings UniqueNameSettings => Realm?.UniqueNameSettings ?? Configuration.UniqueNameSettings;
  public IPasswordSettings PasswordSettings => Realm?.PasswordSettings ?? Configuration.PasswordSettings;
  public bool RequireUniqueEmail => Realm?.RequireUniqueEmail ?? false;
  public bool RequireConfirmedAccount => Realm?.RequireConfirmedAccount ?? false;
}
