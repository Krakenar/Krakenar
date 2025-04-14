using FluentValidation;
using Krakenar.Contracts.Configurations;
using Krakenar.Core.Caching;
using Krakenar.Core.Configurations.Validators;
using Krakenar.Core.Settings;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations.Commands;

public record UpdateConfiguration(UpdateConfigurationPayload Payload) : ICommand<ConfigurationDto>;

/// <exception cref="ValidationException"></exception>
public class UpdateConfigurationHandler : ICommandHandler<UpdateConfiguration, ConfigurationDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ICacheService CacheService { get; }
  protected virtual IConfigurationRepository ConfigurationRepository { get; }

  public UpdateConfigurationHandler(IApplicationContext applicationContext, ICacheService cacheService, IConfigurationRepository configurationRepository)
  {
    ApplicationContext = applicationContext;
    CacheService = cacheService;
    ConfigurationRepository = configurationRepository;
  }

  public virtual async Task<ConfigurationDto> HandleAsync(UpdateConfiguration command, CancellationToken cancellationToken)
  {
    UpdateConfigurationPayload payload = command.Payload;
    new UpdateConfigurationValidator().ValidateAndThrow(payload);

    Configuration configuration = await ConfigurationRepository.LoadAsync(cancellationToken) ?? throw new InvalidOperationException("The configuration was not loaded");

    if (payload.UniqueNameSettings is not null)
    {
      configuration.UniqueNameSettings = new UniqueNameSettings(payload.UniqueNameSettings);
    }
    if (payload.PasswordSettings is not null)
    {
      configuration.PasswordSettings = new PasswordSettings(payload.PasswordSettings);
    }
    if (payload.LoggingSettings is not null)
    {
      configuration.LoggingSettings = new LoggingSettings(payload.LoggingSettings);
    }

    configuration.Update(ApplicationContext.ActorId);

    await ConfigurationRepository.SaveAsync(configuration, cancellationToken);

    return CacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
  }
}
