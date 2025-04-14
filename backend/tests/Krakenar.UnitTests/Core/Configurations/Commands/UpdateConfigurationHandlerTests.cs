using FluentValidation;
using Krakenar.Contracts.Configurations;
using Krakenar.Contracts.Logging;
using Krakenar.Core.Caching;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;
using LoggingSettingsDto = Krakenar.Contracts.Logging.LoggingSettings;
using PasswordSettingsDto = Krakenar.Contracts.Settings.PasswordSettings;
using UniqueNameSettingsDto = Krakenar.Contracts.Settings.UniqueNameSettings;

namespace Krakenar.Core.Configurations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class UpdateConfigurationHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ICacheService> _cacheService = new();
  private readonly Mock<IConfigurationRepository> _configurationRepository = new();

  private readonly UpdateConfigurationHandler _handler;

  public UpdateConfigurationHandlerTests()
  {
    _handler = new(_applicationContext.Object, _cacheService.Object, _configurationRepository.Object);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task Given_InvalidPayload_When_HandleAsync_Then_ValidationException()
  {
    UpdateConfigurationPayload payload = new()
    {
      UniqueNameSettings = new UniqueNameSettingsDto("    "),
      PasswordSettings = new PasswordSettingsDto(0, 1, false, true, true, true, string.Empty),
      LoggingSettings = new LoggingSettingsDto(LoggingExtent.None, onlyErrors: true)
    };
    UpdateConfiguration command = new(payload);
    var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(5, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "UniqueNameSettings.AllowedCharacters");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "GreaterThanOrEqualValidator" && e.PropertyName == "PasswordSettings.RequiredLength");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LessThanOrEqualValidator" && e.PropertyName == "PasswordSettings.RequiredUniqueChars");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "PasswordSettings.HashingStrategy");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EqualValidator" && e.PropertyName == "LoggingSettings.OnlyErrors");
  }

  [Fact(DisplayName = "It should update the existing configuration.")]
  public async Task Given_Version_When_HandleAsync_Then_Updated()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    Configuration configuration = Configuration.Initialize(secret, actorId);
    UniqueNameSettings uniqueNameSettings = configuration.UniqueNameSettings;
    _configurationRepository.Setup(x => x.LoadAsync(_cancellationToken)).ReturnsAsync(configuration);

    ConfigurationDto dto = new();
    _cacheService.SetupGet(x => x.Configuration).Returns(dto);

    UpdateConfigurationPayload payload = new()
    {
      PasswordSettings = new PasswordSettingsDto(6, 1, false, true, true, true, "PBKDF2"),
      LoggingSettings = new LoggingSettingsDto(LoggingExtent.Full, onlyErrors: true)
    };
    UpdateConfiguration command = new(payload);
    ConfigurationDto result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.Same(dto, result);

    Assert.Equal(actorId, configuration.UpdatedBy);
    Assert.Equal(uniqueNameSettings, configuration.UniqueNameSettings);
    Assert.Equal(payload.PasswordSettings, new PasswordSettingsDto(configuration.PasswordSettings));
    Assert.Equal(payload.LoggingSettings, new LoggingSettingsDto(configuration.LoggingSettings));

    _configurationRepository.Verify(x => x.SaveAsync(configuration, _cancellationToken), Times.Once);
  }
}
