using Krakenar.Core.Caching;
using Krakenar.Core.Localization;
using Krakenar.Core.Passwords;
using Krakenar.Core.Tokens;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations.Commands;

public record InitializeConfiguration(string DefaultLocale, string UniqueName, string Password) : ICommand;

public class InitializeConfigurationHandler : ICommandHandler<InitializeConfiguration>
{
  protected virtual ICacheService CacheService { get; }
  protected virtual IConfigurationQuerier ConfigurationQuerier { get; }
  protected virtual IConfigurationRepository ConfigurationRepository { get; }
  protected virtual ILanguageService LanguageService { get; }
  protected virtual IPasswordService PasswordService { get; }
  protected virtual ISecretService SecretService { get; }
  protected virtual IUserService UserService { get; }

  public InitializeConfigurationHandler(
    ICacheService cacheService,
    IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository,
    ILanguageService languageService,
    IPasswordService passwordService,
    ISecretService secretService,
    IUserService userService)
  {
    CacheService = cacheService;
    ConfigurationQuerier = configurationQuerier;
    ConfigurationRepository = configurationRepository;
    LanguageService = languageService;
    PasswordService = passwordService;
    SecretService = secretService;
    UserService = userService;
  }

  public virtual async Task HandleAsync(InitializeConfiguration command, CancellationToken cancellationToken)
  {
    Configuration? configuration = await ConfigurationRepository.LoadAsync(cancellationToken);
    if (configuration is null)
    {
      UserId userId = UserId.NewId();
      ActorId actorId = new(userId.Value);

      Secret secret = SecretService.Generate();
      configuration = Configuration.Initialize(secret, actorId);

      Locale locale = new(command.DefaultLocale);
      Language language = new(locale, isDefault: true, actorId);

      UniqueName uniqueName = new(configuration.UniqueNameSettings, command.UniqueName);
      Password password = PasswordService.ValidateAndHash(command.Password, configuration.PasswordSettings);
      User user = new(uniqueName, password, actorId, userId);

      await ConfigurationRepository.SaveAsync(configuration, cancellationToken); // NOTE(fpion): this should cache the configuration.
      await LanguageService.SaveAsync(language, cancellationToken);
      await UserService.SaveAsync(user, cancellationToken);
    }
    else
    {
      CacheService.Configuration = await ConfigurationQuerier.ReadAsync(configuration, cancellationToken);
    }
  }
}
