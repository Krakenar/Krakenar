using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Logging;
using Krakenar.Core;
using Krakenar.Core.Configurations;
using Krakenar.Core.Configurations.Commands;
using Krakenar.Core.Configurations.Queries;
using Krakenar.Core.Settings;
using Logitar;
using Microsoft.Extensions.DependencyInjection;
using Configuration = Krakenar.Core.Configurations.Configuration;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;
using LoggingSettingsDto = Krakenar.Contracts.Logging.LoggingSettings;
using PasswordSettingsDto = Krakenar.Contracts.Settings.PasswordSettings;
using UniqueNameSettingsDto = Krakenar.Contracts.Settings.UniqueNameSettings;

namespace Krakenar.Configurations;

[Trait(Traits.Category, Categories.Integration)]
public class ConfigurationIntegrationTests : IntegrationTests
{
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IQueryHandler<ReadConfiguration, ConfigurationDto> _readConfiguration;
  private readonly ICommandHandler<ReplaceConfiguration, ConfigurationDto> _replaceConfiguration;
  private readonly ICommandHandler<UpdateConfiguration, ConfigurationDto> _updateConfiguration;

  public ConfigurationIntegrationTests()
  {
    _configurationRepository = ServiceProvider.GetRequiredService<IConfigurationRepository>();
    _readConfiguration = ServiceProvider.GetRequiredService<IQueryHandler<ReadConfiguration, ConfigurationDto>>();
    _replaceConfiguration = ServiceProvider.GetRequiredService<ICommandHandler<ReplaceConfiguration, ConfigurationDto>>();
    _updateConfiguration = ServiceProvider.GetRequiredService<ICommandHandler<UpdateConfiguration, ConfigurationDto>>();
  }

  [Fact(DisplayName = "It should read the configuration.")]
  public async Task Given_Configuration_When_Read_Then_Configuration()
  {
    ReadConfiguration query = new();
    ConfigurationDto configuration = await _readConfiguration.HandleAsync(query);

    Assert.Equal(1, configuration.Version);
    Assert.Equal(Actor, configuration.CreatedBy);
    Assert.Equal(Actor, configuration.UpdatedBy);
    Assert.Equal(configuration.CreatedOn, configuration.UpdatedOn);
    Assert.Equal(DateTime.UtcNow, configuration.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.NotNull(configuration.Secret);
  }

  [Fact(DisplayName = "It should replace the configuration.")]
  public async Task Given_NoVersion_When_Replace_Then_Replaced()
  {
    ReplaceConfigurationPayload payload = new()
    {
      UniqueNameSettings = new UniqueNameSettingsDto(),
      PasswordSettings = new PasswordSettingsDto(6, 1, false, true, true, true, "PBKDF2"),
      LoggingSettings = new LoggingSettingsDto(LoggingExtent.Full, onlyErrors: true)
    };
    ReplaceConfiguration command = new(payload, Version: null);
    ConfigurationDto configuration = await _replaceConfiguration.HandleAsync(command);

    Assert.Equal(2, configuration.Version);
    Assert.Equal(Actor, configuration.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, configuration.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(payload.UniqueNameSettings, configuration.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, configuration.PasswordSettings);
    Assert.Equal(payload.LoggingSettings, configuration.LoggingSettings);
  }

  [Fact(DisplayName = "It should replace the configuration from reference.")]
  public async Task Given_Version_When_Replace_Then_Replaced()
  {
    Configuration? aggregate = await _configurationRepository.LoadAsync();
    Assert.NotNull(aggregate);
    long version = aggregate.Version;

    UniqueNameSettings uniqueNameSettings = new(allowedCharacters: null);
    aggregate.UniqueNameSettings = uniqueNameSettings;
    aggregate.Update(ActorId);
    await _configurationRepository.SaveAsync(aggregate);

    ReplaceConfigurationPayload payload = new()
    {
      PasswordSettings = new PasswordSettingsDto(6, 1, false, true, true, true, "PBKDF2"),
      LoggingSettings = new LoggingSettingsDto(LoggingExtent.Full, onlyErrors: true)
    };
    ReplaceConfiguration command = new(payload, version);
    ConfigurationDto configuration = await _replaceConfiguration.HandleAsync(command);

    Assert.Equal(aggregate.Version + 1, configuration.Version);
    Assert.Equal(Actor, configuration.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, configuration.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(uniqueNameSettings, new UniqueNameSettings(configuration.UniqueNameSettings));
    Assert.Equal(payload.PasswordSettings, configuration.PasswordSettings);
    Assert.Equal(payload.LoggingSettings, configuration.LoggingSettings);
  }

  [Fact(DisplayName = "It should update the configuration.")]
  public async Task Given_Configuration_When_Update_Then_Updated()
  {
    UpdateConfigurationPayload payload = new()
    {
      LoggingSettings = new LoggingSettingsDto(LoggingExtent.Full, onlyErrors: true)
    };
    UpdateConfiguration command = new(payload);
    ConfigurationDto configuration = await _updateConfiguration.HandleAsync(command);

    Assert.Equal(2, configuration.Version);
    Assert.Equal(Actor, configuration.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, configuration.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(payload.LoggingSettings, configuration.LoggingSettings);
  }
}
