using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Logging;
using Krakenar.Core.Configurations;
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
  private readonly IConfigurationService _configurationService;

  public ConfigurationIntegrationTests() : base()
  {
    _configurationRepository = ServiceProvider.GetRequiredService<IConfigurationRepository>();
    _configurationService = ServiceProvider.GetRequiredService<IConfigurationService>();
  }

  [Fact(DisplayName = "It should read the configuration.")]
  public async Task Given_Configuration_When_Read_Then_Configuration()
  {
    ConfigurationDto configuration = await _configurationService.ReadAsync();

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
    ConfigurationDto configuration = await _configurationService.ReplaceAsync(payload);

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
    ConfigurationDto configuration = await _configurationService.ReplaceAsync(payload, version);

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
    ConfigurationDto configuration = await _configurationService.UpdateAsync(payload);

    Assert.Equal(2, configuration.Version);
    Assert.Equal(Actor, configuration.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, configuration.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(payload.LoggingSettings, configuration.LoggingSettings);
  }
}
