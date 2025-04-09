using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;

namespace Krakenar.Core.Actors;

public interface IActorService
{
  Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}
