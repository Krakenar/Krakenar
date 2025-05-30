﻿using FluentValidation;
using Krakenar.Core.Caching;
using Krakenar.Core.Localization;
using Krakenar.Core.Passwords;
using Krakenar.Core.Tokens;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Configurations.Commands;

public record InitializeConfiguration(string DefaultLocale, string UniqueName, string Password) : ICommand;

/// <exception cref="LocaleAlreadyUsedException"></exception>
/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class InitializeConfigurationHandler : ICommandHandler<InitializeConfiguration>
{
  protected virtual ICacheService CacheService { get; }
  protected virtual IConfigurationQuerier ConfigurationQuerier { get; }
  protected virtual IConfigurationRepository ConfigurationRepository { get; }
  protected virtual ILanguageManager LanguageManager { get; }
  protected virtual IPasswordManager PasswordManager { get; }
  protected virtual ISecretManager SecretManager { get; }
  protected virtual IUserManager UserManager { get; }

  public InitializeConfigurationHandler(
    ICacheService cacheService,
    IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository,
    ILanguageManager languageManager,
    IPasswordManager passwordManager,
    ISecretManager secretManager,
    IUserManager userManager)
  {
    CacheService = cacheService;
    ConfigurationQuerier = configurationQuerier;
    ConfigurationRepository = configurationRepository;
    LanguageManager = languageManager;
    PasswordManager = passwordManager;
    SecretManager = secretManager;
    UserManager = userManager;
  }

  public virtual async Task HandleAsync(InitializeConfiguration command, CancellationToken cancellationToken)
  {
    Configuration? configuration = await ConfigurationRepository.LoadAsync(cancellationToken);
    if (configuration is null)
    {
      UserId userId = UserId.NewId();
      ActorId actorId = new(userId.Value);

      Secret secret = SecretManager.Generate();
      configuration = Configuration.Initialize(secret, actorId);

      Locale locale = new(command.DefaultLocale);
      Language language = new(locale, isDefault: true, actorId);

      UniqueName uniqueName = new(configuration.UniqueNameSettings, command.UniqueName);
      Password password = PasswordManager.ValidateAndHash(command.Password, configuration.PasswordSettings);
      User user = new(uniqueName, password, actorId, userId);

      await ConfigurationRepository.SaveAsync(configuration, cancellationToken); // NOTE(fpion): this should cache the configuration.
      await LanguageManager.SaveAsync(language, cancellationToken);
      await UserManager.SaveAsync(user, cancellationToken);
    }
    CacheService.Configuration = await ConfigurationQuerier.ReadAsync(configuration, cancellationToken);
  }
}
