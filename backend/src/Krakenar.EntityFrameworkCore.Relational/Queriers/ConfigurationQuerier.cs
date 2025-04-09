using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Logging;
using Krakenar.Core.Actors;
using Krakenar.Core.Configurations;
using Logitar;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;
using ConfigurationEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Configuration;

namespace Krakenar.EntityFrameworkCore.Relational.Queriers;

public class ConfigurationQuerier : IConfigurationQuerier
{
  protected virtual IActorService ActorService { get; }
  protected virtual DbSet<ConfigurationEntity> Configuration { get; }

  public ConfigurationQuerier(IActorService actorService, KrakenarContext context)
  {
    ActorService = actorService;
    Configuration = context.Configuration;
  }

  public virtual async Task<ConfigurationDto> ReadAsync(CancellationToken cancellationToken)
  {
    ConfigurationEntity[] configurations = await Configuration.AsNoTracking()
      .OrderBy(x => x.Version)
      .ToArrayAsync(cancellationToken);

    ConfigurationDto dto = new();
    if (configurations.Length > 0)
    {
      foreach (ConfigurationEntity configuration in configurations)
      {
        switch (configuration.Key)
        {
          case "Secret":
            dto.Secret = configuration.Value;
            break;
          case "UniqueNameSettings.AllowedCharacters":
            dto.UniqueNameSettings.AllowedCharacters = configuration.Value;
            break;
          case "PasswordSettings.RequiredLength":
            dto.PasswordSettings.RequiredLength = int.Parse(configuration.Value);
            break;
          case "PasswordSettings.RequiredUniqueChars":
            dto.PasswordSettings.RequiredUniqueChars = int.Parse(configuration.Value);
            break;
          case "PasswordSettings.RequireNonAlphanumeric":
            dto.PasswordSettings.RequireNonAlphanumeric = bool.Parse(configuration.Value);
            break;
          case "PasswordSettings.RequireLowercase":
            dto.PasswordSettings.RequireLowercase = bool.Parse(configuration.Value);
            break;
          case "PasswordSettings.RequireUppercase":
            dto.PasswordSettings.RequireUppercase = bool.Parse(configuration.Value);
            break;
          case "PasswordSettings.RequireDigit":
            dto.PasswordSettings.RequireDigit = bool.Parse(configuration.Value);
            break;
          case "PasswordSettings.HashingStrategy":
            dto.PasswordSettings.HashingStrategy = configuration.Value;
            break;
          case "LoggingSettings.Extent":
            dto.LoggingSettings.Extent = Enum.Parse<LoggingExtent>(configuration.Value);
            break;
          case "LoggingSettings.OnlyErrors":
            dto.LoggingSettings.OnlyErrors = bool.Parse(configuration.Value);
            break;
        }
      }

      HashSet<ActorId> actorIds = new(capacity: 2);

      ConfigurationEntity first = configurations.First();
      if (first.CreatedBy is not null)
      {
        actorIds.Add(new ActorId(first.CreatedBy));
      }
      dto.CreatedOn = first.CreatedOn.AsUniversalTime();

      ConfigurationEntity last = configurations.Last();
      dto.Version = last.Version;
      if (last.UpdatedBy is not null)
      {
        actorIds.Add(new ActorId(last.UpdatedBy));
      }
      dto.UpdatedOn = last.UpdatedOn.AsUniversalTime();

      if (actorIds.Count > 0)
      {
        IReadOnlyDictionary<ActorId, Actor> actors = await ActorService.FindAsync(actorIds, cancellationToken);
        if (first.CreatedBy is not null)
        {
          ActorId createdById = new(first.CreatedBy);
          if (actors.TryGetValue(createdById, out Actor? createdBy))
          {
            dto.CreatedBy = createdBy;
          }
        }
        if (last.UpdatedBy is not null)
        {
          ActorId updatedById = new(last.UpdatedBy);
          if (actors.TryGetValue(updatedById, out Actor? updatedBy))
          {
            dto.UpdatedBy = updatedBy;
          }
        }
      }
    }

    return dto;
  }

  public virtual async Task<ConfigurationDto> ReadAsync(Configuration configuration, CancellationToken cancellationToken)
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
