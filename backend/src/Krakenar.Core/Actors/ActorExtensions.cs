using Krakenar.Contracts.Actors;
using Krakenar.Core.Realms;
using Krakenar.Core.Users;
using Logitar;
using Logitar.EventSourcing;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Actors;

public static class ActorExtensions
{
  private const char Separator = ':';

  public static ActorId GetActorId(this Actor actor) // TODO(fpion): will not work when in a Realm!
  {
    if (actor.Type == ActorType.System)
    {
      throw new ArgumentException($"The actor type cannot be {ActorType.System}.", nameof(actor));
    }
    string encoded = Convert.ToBase64String(actor.Id.ToByteArray()).ToUriSafeBase64();
    return new ActorId(string.Join(Separator, actor.Type, encoded));
  }

  public static ActorId GetActorId(this UserDto user)
  {
    RealmId? realmId = user.Realm is null ? null : new(user.Realm.Id);
    UserId userId = new(user.Id, realmId);
    return new ActorId(userId.Value);
  }
}
