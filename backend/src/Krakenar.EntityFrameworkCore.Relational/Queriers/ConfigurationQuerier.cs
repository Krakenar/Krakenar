using Krakenar.Contracts.Actors;
using Krakenar.Core.Actors;
using Krakenar.Core.Configurations;
using Logitar.EventSourcing;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class ConfigurationQuerier : IConfigurationQuerier
{
  protected virtual IActorService ActorService { get; }

  public ConfigurationQuerier(IActorService actorService)
  {
    ActorService = actorService;
  }

  public Task<ConfigurationDto> ReadAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException(); // TODO(fpion): implement
  }

  public async Task<ConfigurationDto> ReadAsync(Configuration configuration, CancellationToken cancellationToken)
  {
    HashSet<ActorId> actorIds = new(capacity: 2);
    if (configuration.CreatedBy.HasValue)
    {
      actorIds.Add(configuration.CreatedBy.Value);
    }
    if (configuration.UpdatedBy.HasValue)
    {
      actorIds.Add(configuration.UpdatedBy.Value);
    }
    IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);
    return mapper.ToConfiguration(configuration);
  }
}
