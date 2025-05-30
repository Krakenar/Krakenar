using Krakenar.Core.Configurations.Events;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;

namespace Krakenar.Core.Configurations;

[Trait(Traits.Category, Categories.Unit)]
public class ConfigurationTests
{
  private readonly Configuration _configuration;

  public ConfigurationTests()
  {
    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    ActorId actorId = ActorId.NewId();
    _configuration = Configuration.Initialize(secret, actorId);
  }

  [Fact(DisplayName = "It should handle LoggingSettings change correctly.")]
  public void Given_Changes_When_LoggingSettings_Then_Changed()
  {
    LoggingSettings loggingSettings = new(Contracts.Logging.LoggingExtent.Full, onlyErrors: true);
    _configuration.LoggingSettings = loggingSettings;
    _configuration.Update(_configuration.CreatedBy);
    Assert.Equal(loggingSettings, _configuration.LoggingSettings);
    Assert.Contains(_configuration.Changes, change => change is ConfigurationUpdated updated && updated.LoggingSettings is not null && updated.LoggingSettings.Equals(loggingSettings));

    _configuration.ClearChanges();

    _configuration.LoggingSettings = loggingSettings;
    _configuration.Update();
    Assert.False(_configuration.HasChanges);
  }

  [Fact(DisplayName = "It should handle PasswordSettings change correctly.")]
  public void Given_Changes_When_PasswordSettings_Then_Changed()
  {
    PasswordSettings passwordSettings = new(6, 1, false, true, true, true, "PBKDF2");
    _configuration.PasswordSettings = passwordSettings;
    _configuration.Update(_configuration.CreatedBy);
    Assert.Equal(passwordSettings, _configuration.PasswordSettings);
    Assert.Contains(_configuration.Changes, change => change is ConfigurationUpdated updated && updated.PasswordSettings is not null && updated.PasswordSettings.Equals(passwordSettings));

    _configuration.ClearChanges();

    _configuration.PasswordSettings = passwordSettings;
    _configuration.Update();
    Assert.False(_configuration.HasChanges);
  }

  [Fact(DisplayName = "It should handle UniqueNameSettings change correctly.")]
  public void Given_Changes_When_UniqueNameSettings_Then_Changed()
  {
    UniqueNameSettings uniqueNameSettings = new(allowedCharacters: null);
    _configuration.UniqueNameSettings = uniqueNameSettings;
    _configuration.Update(_configuration.CreatedBy);
    Assert.Equal(uniqueNameSettings, _configuration.UniqueNameSettings);
    Assert.Contains(_configuration.Changes, change => change is ConfigurationUpdated updated && updated.UniqueNameSettings is not null && updated.UniqueNameSettings.Equals(uniqueNameSettings));

    _configuration.ClearChanges();

    _configuration.UniqueNameSettings = uniqueNameSettings;
    _configuration.Update();
    Assert.False(_configuration.HasChanges);
  }

  [Fact(DisplayName = "It should initialize a new configuration.")]
  public void Given_Arguments_When_Initialize_Then_Initialized()
  {
    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    ActorId actorId = ActorId.NewId();

    Configuration configuration = Configuration.Initialize(secret, actorId);

    Assert.Equal("Configuration", configuration.Id.Value);
    Assert.Equal(1, configuration.Version);
    Assert.Equal(actorId, configuration.CreatedBy);
    Assert.Equal(actorId, configuration.UpdatedBy);

    Assert.Equal(secret, configuration.Secret);
    Assert.Equal(new UniqueNameSettings(), configuration.UniqueNameSettings);
    Assert.Equal(new PasswordSettings(), configuration.PasswordSettings);
    Assert.Equal(new LoggingSettings(), configuration.LoggingSettings);

    Assert.Contains(configuration.Changes, change => change is ConfigurationInitialized);
  }

  [Fact(DisplayName = "SetSecret: it should handle Secret change correctly.")]
  public void Given_Changes_When_SetSecret_Then_Changed()
  {
    ActorId actorId = ActorId.NewId();

    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    _configuration.SetSecret(secret, actorId);
    Assert.Equal(secret, _configuration.Secret);
    Assert.Equal(actorId, _configuration.SecretChangedBy);
    Assert.Equal(DateTime.UtcNow, _configuration.SecretChangedOn.AsUniversalTime(), TimeSpan.FromSeconds(1));
    Assert.Contains(_configuration.Changes, change => change is ConfigurationSecretChanged changed && changed.ActorId == actorId && changed.Secret.Equals(secret));

    _configuration.ClearChanges();

    _configuration.SetSecret(secret, actorId);
    Assert.False(_configuration.HasChanges);
  }
}
