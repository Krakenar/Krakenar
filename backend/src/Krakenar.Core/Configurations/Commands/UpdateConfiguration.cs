using FluentValidation;
using Krakenar.Contracts.Configurations;
using Krakenar.Core.Caching;
using Krakenar.Core.Configurations.Validators;
using Krakenar.Core.Logging;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar;
using Logitar.CQRS;
using Logitar.EventSourcing;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations.Commands;

public record UpdateConfiguration(UpdateConfigurationPayload Payload) : IAnonymizable, ICommand<ConfigurationDto>
{
  public object? Anonymize()
  {
    if (string.IsNullOrWhiteSpace(Payload.Secret?.Value))
    {
      return this;
    }

    UpdateConfiguration clone = this.DeepClone();
    clone.Payload.Secret = new Contracts.Change<string>(Payload.Secret.Value.Mask());
    return clone;
  }
}

/// <exception cref="ValidationException"></exception>
public class UpdateConfigurationHandler : ICommandHandler<UpdateConfiguration, ConfigurationDto>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ICacheService CacheService { get; }
  protected virtual IConfigurationRepository ConfigurationRepository { get; }
  protected virtual ISecretManager SecretManager { get; }

  public UpdateConfigurationHandler(
    IApplicationContext applicationContext,
    ICacheService cacheService,
    IConfigurationRepository configurationRepository,
    ISecretManager secretManager)
  {
    ApplicationContext = applicationContext;
    CacheService = cacheService;
    ConfigurationRepository = configurationRepository;
    SecretManager = secretManager;
  }

  public virtual async Task<ConfigurationDto> HandleAsync(UpdateConfiguration command, CancellationToken cancellationToken)
  {
    UpdateConfigurationPayload payload = command.Payload;
    new UpdateConfigurationValidator().ValidateAndThrow(payload);

    Configuration configuration = await ConfigurationRepository.LoadAsync(cancellationToken) ?? throw new InvalidOperationException("The configuration was not loaded");

    ActorId? actorId = ApplicationContext.ActorId;

    if (payload.Secret is not null)
    {
      Secret secret = string.IsNullOrWhiteSpace(payload.Secret.Value) ? SecretManager.Generate() : SecretManager.Encrypt(payload.Secret.Value);
      configuration.SetSecret(secret, actorId);
    }

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

    configuration.Update(actorId);

    await ConfigurationRepository.SaveAsync(configuration, cancellationToken);

    return CacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
  }
}
