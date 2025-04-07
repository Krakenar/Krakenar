using Krakenar.Core.Caching;
using Krakenar.Core.Localization;
using Krakenar.Core.Passwords;
using Krakenar.Core.Settings;
using Krakenar.Core.Tokens;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Logitar.Security.Cryptography;
using Moq;
using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class InitializeConfigurationHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ICacheService> _cacheService = new();
  private readonly Mock<IConfigurationQuerier> _configurationQuerier = new();
  private readonly Mock<IConfigurationRepository> _configurationRepository = new();
  private readonly Mock<ILanguageService> _languageService = new();
  private readonly Mock<IPasswordService> _passwordService = new();
  private readonly Mock<ISecretService> _secretService = new();
  private readonly Mock<IUserService> _userService = new();

  private readonly InitializeConfigurationHandler _handler;

  public InitializeConfigurationHandlerTests()
  {
    _handler = new(_cacheService.Object, _configurationQuerier.Object, _configurationRepository.Object, _languageService.Object, _passwordService.Object, _secretService.Object, _userService.Object);
  }

  [Fact(DisplayName = "It should cache an initialized configuration.")]
  public async Task Given_Initialized_When_HandleAsync_Then_Cached()
  {
    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    ActorId actorId = ActorId.NewId();
    Configuration configuration = Configuration.Initialize(secret, actorId);
    _configurationRepository.Setup(x => x.LoadAsync(_cancellationToken)).ReturnsAsync(configuration);

    ConfigurationDto dto = new();
    _configurationQuerier.Setup(x => x.ReadAsync(configuration, _cancellationToken)).ReturnsAsync(dto);

    InitializeConfiguration command = new("en", "admin", "P@s$W0rD");
    await _handler.HandleAsync(command, _cancellationToken);

    _cacheService.VerifySet(x => x.Configuration = dto, Times.Once());
  }

  [Fact(DisplayName = "It should initialize a new configuration.")]
  public async Task Given_NotInitialized_When_HandleAsync_Then_Initialized()
  {
    InitializeConfiguration command = new("en", "admin", "P@s$W0rD");

    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    _secretService.Setup(x => x.Generate(null)).Returns(secret);

    Base64Password password = new(command.Password);
    _passwordService.Setup(x => x.ValidateAndHash(command.Password, It.Is<PasswordSettings>(s => s.Equals(new PasswordSettings())))).Returns(password);

    await _handler.HandleAsync(command, _cancellationToken);

    _configurationRepository.Verify(x => x.SaveAsync(
      It.Is<Configuration>(c => c.CreatedBy.HasValue && c.UpdatedBy.HasValue && c.CreatedBy.Equals(c.UpdatedBy)
        && c.Secret.Equals(secret)
        && c.UniqueNameSettings.Equals(new UniqueNameSettings())
        && c.PasswordSettings.Equals(new PasswordSettings())
        && c.LoggingSettings.Equals(new LoggingSettings())),
      _cancellationToken), Times.Once());

    _languageService.Verify(x => x.SaveAsync(
      It.Is<Language>(l => l.CreatedBy.HasValue && l.UpdatedBy.HasValue && l.CreatedBy.Equals(l.UpdatedBy)
        && !l.RealmId.HasValue && !l.EntityId.Equals(Guid.Empty)
        && l.IsDefault && l.Locale.Code == command.DefaultLocale),
      _cancellationToken), Times.Once());

    _userService.Verify(x => x.SaveAsync(
      It.Is<User>(u => u.CreatedBy.HasValue && u.UpdatedBy.HasValue && u.CreatedBy.Equals(u.UpdatedBy) && u.CreatedBy.Value.Value.Equals(u.Id.Value)
        && !u.RealmId.HasValue && !u.EntityId.Equals(Guid.Empty)
        && u.UniqueName.Value.Equals(command.UniqueName) && u.HasPassword),
      _cancellationToken), Times.Once());
  }
}
