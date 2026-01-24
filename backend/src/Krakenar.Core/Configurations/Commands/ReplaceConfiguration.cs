using FluentValidation;
using Krakenar.Contracts.Configurations;
using Krakenar.Core.Caching;
using Krakenar.Core.Configurations.Validators;
using Krakenar.Core.Settings;
using Logitar.CQRS;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations.Commands;

public record ReplaceConfiguration(ReplaceConfigurationPayload Payload, long? Version) : ICommand<ConfigurationDto>;

/// <exception cref="ValidationException"></exception>
public class ReplaceConfigurationHandler : ICommandHandler<ReplaceConfiguration, ConfigurationDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ICacheService CacheService { get; }
  protected virtual IConfigurationRepository ConfigurationRepository { get; }

  public ReplaceConfigurationHandler(IApplicationContext applicationContext, ICacheService cacheService, IConfigurationRepository configurationRepository)
  {
    ApplicationContext = applicationContext;
    CacheService = cacheService;
    ConfigurationRepository = configurationRepository;
  }

  public virtual async Task<ConfigurationDto> HandleAsync(ReplaceConfiguration command, CancellationToken cancellationToken)
  {
    ReplaceConfigurationPayload payload = command.Payload;
    new ReplaceConfigurationValidator().ValidateAndThrow(payload);

    Configuration configuration = await ConfigurationRepository.LoadAsync(cancellationToken) ?? throw new InvalidOperationException("The configuration was not loaded");

    Configuration reference = (command.Version.HasValue
      ? await ConfigurationRepository.LoadAsync(command.Version, cancellationToken)
      : null) ?? configuration;

    UniqueNameSettings uniqueNameSettings = new(payload.UniqueNameSettings);
    if (reference.UniqueNameSettings != uniqueNameSettings)
    {
      configuration.UniqueNameSettings = uniqueNameSettings;
    }
    PasswordSettings passwordSettings = new(payload.PasswordSettings);
    if (reference.PasswordSettings != passwordSettings)
    {
      configuration.PasswordSettings = passwordSettings;
    }
    LoggingSettings loggingSettings = new(payload.LoggingSettings);
    if (reference.LoggingSettings != loggingSettings)
    {
      configuration.LoggingSettings = loggingSettings;
    }

    configuration.Update(ApplicationContext.ActorId);

    await ConfigurationRepository.SaveAsync(configuration, cancellationToken);

    return CacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
  }
}
