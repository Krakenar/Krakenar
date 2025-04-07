using Krakenar.Contracts.Actors;
using Krakenar.Core.Actors;
using Krakenar.Core.Caching;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;

namespace Krakenar.EntityFrameworkCore.Relational.Actors;

public class ActorService : IActorService
{
  protected virtual ICacheService CacheService { get; }
  protected virtual KrakenarContext Context { get; }

  public ActorService(ICacheService cacheService, KrakenarContext context)
  {
    CacheService = cacheService;
    Context = context;
  }

  public virtual async Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    int capacity = ids.Count();
    Dictionary<ActorId, Actor> actors = new(capacity);
    HashSet<string> missingKeys = new(capacity);

    foreach (ActorId id in ids)
    {
      Actor? actor = CacheService.GetActor(id);
      if (actor is null)
      {
        missingKeys.Add(id.ToString());
      }
      else
      {
        actors[id] = actor;
      }
    }

    if (missingKeys.Count > 0)
    {
      ActorEntity[] entities = await Context.Actors.AsNoTracking()
        .Where(x => missingKeys.Contains(x.Key))
        .ToArrayAsync(cancellationToken);

      Mapper mapper = new();
      foreach (ActorEntity entity in entities)
      {
        ActorId id = new(entity.Key);
        Actor actor = mapper.ToActor(entity);
        actors[id] = actor;
      }
    }

    foreach (Actor actor in actors.Values)
    {
      CacheService.SetActor(actor);
    }

    return actors.AsReadOnly();
  }
}
