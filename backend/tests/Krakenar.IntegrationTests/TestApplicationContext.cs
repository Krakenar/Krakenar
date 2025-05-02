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
  public string BaseUrl { get; set; } = "http://localhost:80";
  public ConfigurationDto? Configuration { get; set; }
  public RealmDto? Realm { get; set; }
  public RealmId? RealmId { get; set; }

  public IUniqueNameSettings UniqueNameSettings => Realm?.UniqueNameSettings ?? Configuration?.UniqueNameSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
  public IPasswordSettings PasswordSettings => Realm?.PasswordSettings ?? Configuration?.PasswordSettings ?? throw new InvalidOperationException("The configuration has not been initialized.");
  public bool RequireUniqueEmail => Realm?.RequireUniqueEmail ?? false;
  public bool RequireConfirmedAccount => Realm?.RequireConfirmedAccount ?? false;
}
