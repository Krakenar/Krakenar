using Krakenar.Contracts.Actors;
using Krakenar.Core.ApiKeys;
using Krakenar.Core.Realms;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Actors;

public static class ActorExtensions
{
  public static ActorId GetActorId(this Actor actor)
  {
    RealmId? realmId = actor.RealmId.HasValue ? new RealmId(actor.RealmId.Value) : null;
    switch (actor.Type)
    {
      case ActorType.ApiKey:
        ApiKeyId apiKeyId = new(actor.Id, realmId);
        return new ActorId(apiKeyId.Value);
      case ActorType.User:
        UserId userId = new(actor.Id, realmId);
        return new ActorId(userId.Value);
      default:
        throw new ArgumentException($"The actor type cannot be {actor.Type}.", nameof(actor));
    }
  }
}
