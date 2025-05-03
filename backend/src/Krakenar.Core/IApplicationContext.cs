using Krakenar.Contracts.Settings;
using Krakenar.Core.Realms;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core;

public interface IApplicationContext
{
  ActorId? ActorId { get; }

  string BaseUrl { get; }

  RealmDto? Realm { get; }
  RealmId? RealmId { get; }

  Secret Secret { get; }
  IUniqueNameSettings UniqueNameSettings { get; }
  IPasswordSettings PasswordSettings { get; }
  bool RequireUniqueEmail { get; }
  bool RequireConfirmedAccount { get; }
}
