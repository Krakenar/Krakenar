using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.Core.Actors;

public static class ActorExtensions
{
  private const char Separator = ':';

  public static ActorId GetActorId(this Actor actor)
  {
    if (actor.Type == ActorType.System)
    {
      throw new ArgumentException($"The actor type cannot be {ActorType.System}.", nameof(actor));
    }
    string encoded = Convert.ToBase64String(actor.Id.ToByteArray()).ToUriSafeBase64();
    return new ActorId(string.Join(Separator, actor.Type, encoded));
  }
}
