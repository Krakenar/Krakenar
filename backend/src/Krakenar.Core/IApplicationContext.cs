using Krakenar.Contracts.Settings;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core;

public interface IApplicationContext
{
  ActorId? ActorId { get; }
  RealmDto? Realm { get; }
  RealmId? RealmId { get; }

  IUniqueNameSettings UniqueNameSettings { get; }
}
