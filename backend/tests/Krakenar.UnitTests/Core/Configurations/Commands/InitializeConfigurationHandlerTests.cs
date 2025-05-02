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
  private readonly Mock<ILanguageManager> _languageManager = new();
  private readonly Mock<IPasswordService> _passwordService = new();
  private readonly Mock<ISecretManager> _secretManager = new();
  private readonly Mock<IUserManager> _userManager = new();

  private readonly InitializeConfigurationHandler _handler;

  public InitializeConfigurationHandlerTests()
  {
    _handler = new(_cacheService.Object, _configurationQuerier.Object, _configurationRepository.Object, _languageManager.Object, _passwordService.Object, _secretManager.Object, _userManager.Object);
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

    _cacheService.VerifySet(x => x.Configuration = dto, Times.Once);
  }

  [Fact(DisplayName = "It should initialize a new configuration.")]
  public async Task Given_NotInitialized_When_HandleAsync_Then_Initialized()
  {
    InitializeConfiguration command = new("en", "admin", "P@s$W0rD");

    Secret secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));
    _secretManager.Setup(x => x.Generate(null)).Returns(secret);

    Base64Password password = new(command.Password);
    _passwordService.Setup(x => x.ValidateAndHash(command.Password, It.Is<PasswordSettings>(s => s.Equals(new PasswordSettings())))).Returns(password);

    Configuration? configuration = null;
    _configurationRepository.Setup(x => x.SaveAsync(It.IsAny<Configuration>(), _cancellationToken)).Callback<Configuration, CancellationToken>((c, _) => configuration = c);

    Language? language = null;
    _languageManager.Setup(x => x.SaveAsync(It.IsAny<Language>(), _cancellationToken)).Callback<Language, CancellationToken>((l, _) => language = l);

    User? user = null;
    _userManager.Setup(x => x.SaveAsync(It.IsAny<User>(), _cancellationToken)).Callback<User, CancellationToken>((u, _) => user = u);

    await _handler.HandleAsync(command, _cancellationToken);

    _cacheService.VerifySet(x => x.Configuration = It.IsAny<ConfigurationDto>(), Times.Once);

    Assert.NotNull(user);
    Assert.Equal(user.Id.Value, user.CreatedBy?.Value);
    Assert.Equal(user.Id.Value, user.UpdatedBy?.Value);
    Assert.Null(user.RealmId);
    Assert.NotEqual(Guid.Empty, user.EntityId);
    Assert.Equal(command.UniqueName, user.UniqueName.Value);
    Assert.True(user.HasPassword);

    Assert.NotNull(language);
    Assert.Equal(user.Id.Value, language.CreatedBy?.Value);
    Assert.Equal(user.Id.Value, language.UpdatedBy?.Value);
    Assert.Null(language.RealmId);
    Assert.NotEqual(Guid.Empty, language.EntityId);
    Assert.True(language.IsDefault);
    Assert.Equal(command.DefaultLocale, language.Locale.Code);

    Assert.NotNull(configuration);
    Assert.Equal(user.Id.Value, configuration.CreatedBy?.Value);
    Assert.Equal(user.Id.Value, configuration.UpdatedBy?.Value);
    Assert.Equal(secret, configuration.Secret);
    Assert.Equal(new UniqueNameSettings(), configuration.UniqueNameSettings);
    Assert.Equal(new PasswordSettings(), configuration.PasswordSettings);
    Assert.Equal(new LoggingSettings(), configuration.LoggingSettings);
  }
}
