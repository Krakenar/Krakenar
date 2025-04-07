using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Logging;
using Krakenar.Contracts.Settings;
using Logitar;
using Logitar.EventSourcing;
using ActorEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Actor;
using ConfigurationAggregate = Krakenar.Core.Configurations.Configuration;

namespace Krakenar.EntityFrameworkCore.Relational;

public class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors)
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public virtual Actor ToActor(ActorEntity source) => new(source.DisplayName)
  {
    Type = source.Type,
    Id = source.Id,
    IsDeleted = source.IsDeleted,
    EmailAddress = source.EmailAddress,
    PictureUrl = source.PictureUrl
  };

  public Configuration ToConfiguration(ConfigurationAggregate source)
  {
    Configuration destination = new()
    {
      Secret = source.Secret.Value,
      UniqueNameSettings = new UniqueNameSettings(source.UniqueNameSettings),
      PasswordSettings = new PasswordSettings(source.PasswordSettings),
      LoggingSettings = new LoggingSettings(source.LoggingSettings)
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateRoot source, Aggregate destination)
  {
    destination.Version = source.Version;

    destination.CreatedBy = TryFindActor(source.CreatedBy) ?? _system;
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = TryFindActor(source.UpdatedBy) ?? _system;
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor? TryFindActor(ActorId? id) => id.HasValue && _actors.TryGetValue(id.Value, out Actor? actor) ? actor : null;
}
