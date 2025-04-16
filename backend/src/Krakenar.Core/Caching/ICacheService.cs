using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Configurations;
using Logitar.EventSourcing;

namespace Krakenar.Core.Caching;

public interface ICacheService
{
  Configuration? Configuration { get; set; }

  Actor? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(Actor actor);
}
