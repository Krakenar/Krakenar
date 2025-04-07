using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core;

public interface IApplicationContext // TODO(fpion): implement
{
  ActorId? ActorId { get; }
  RealmDto? Realm { get; }
  RealmId? RealmId { get; }
}
