using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Configurations;
using Krakenar.Core.Actors;
using Krakenar.Core.Caching;
using Krakenar.Infrastructure.Settings;
using Logitar.EventSourcing;
using Microsoft.Extensions.Caching.Memory;

namespace Krakenar.Infrastructure.Caching;

public class CacheService : ICacheService
{
  protected virtual IMemoryCache MemoryCache { get; }
  protected virtual CachingSettings Settings { get; }

  public CacheService(IMemoryCache memoryCache, CachingSettings settings)
  {
    MemoryCache = memoryCache;
    Settings = settings;
  }

  public virtual Configuration? Configuration
  {
    get => MemoryCache.TryGetValue(ConfigurationKey, out Configuration? configuration) ? configuration : null;
    set
    {
      if (value is null)
      {
        MemoryCache.Remove(ConfigurationKey);
      }
      else
      {
        MemoryCache.Set(ConfigurationKey, value);
      }
    }
  }
  protected virtual string ConfigurationKey => "Configuration";

  public virtual Actor? GetActor(ActorId id)
  {
    string key = GetActorKey(id);
    return MemoryCache.TryGetValue(key, out Actor? actor) ? actor : null;
  }
  public virtual void RemoveActor(ActorId id)
  {
    string key = GetActorKey(id);
    MemoryCache.Remove(key);
  }
  public virtual void SetActor(Actor actor)
  {
    ActorId id = actor.GetActorId();
    string key = GetActorKey(id);
    MemoryCache.Set(key, actor, Settings.ActorLifetime);
  }
  protected virtual string GetActorKey(ActorId id) => $"Actor.Id={id}";
}
