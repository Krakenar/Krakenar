using Krakenar.Core.Caching;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Commands;
using Krakenar.Core.Passwords;
using Krakenar.Core.Tokens;
using Krakenar.Core.Users;
using Krakenar.Core.Users.Commands;
using Logitar.EventSourcing;
using MediatR;

namespace Krakenar.Core.Configurations.Commands;

public record InitializeConfigurationCommand(string DefaultLocale, string UniqueName, string Password) : IRequest;

internal class InitializeConfigurationCommandHandler : IRequestHandler<InitializeConfigurationCommand>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IMediator _mediator;
  private readonly IPasswordManager _passwordManager;
  private readonly ISecretHelper _secretHelper;

  public InitializeConfigurationCommandHandler(
    ICacheService cacheService,
    IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository,
    IMediator mediator,
    IPasswordManager passwordManager,
    ISecretHelper secretHelper)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
    _mediator = mediator;
    _passwordManager = passwordManager;
    _secretHelper = secretHelper;
  }

  public async Task Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    Configuration? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration is null)
    {
      UserId userId = UserId.NewId(realmId: null);
      ActorId actorId = new(userId.Value);

      Secret secret = _secretHelper.Generate();
      configuration = Configuration.Initialize(secret, actorId);

      Locale locale = new(command.DefaultLocale);
      Language language = new(locale, isDefault: true, actorId);

      UniqueName uniqueName = new(configuration.UniqueNameSettings, command.UniqueName);
      Password password = _passwordManager.ValidateAndHash(configuration.PasswordSettings, command.Password);
      User user = new(uniqueName, password, actorId, userId)
      {
        Locale = locale
      };
      user.Update(actorId);

      await _configurationRepository.SaveAsync(configuration, cancellationToken); // NOTE(fpion): this should cache the configuration.
      await _mediator.Send(new SaveLanguageCommand(language), cancellationToken);
      await _mediator.Send(new SaveUserCommand(user), cancellationToken);
    }
    else
    {
      _cacheService.Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
    }
  }
}
