using Krakenar.Core.Caching;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Commands;
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

  public InitializeConfigurationCommandHandler(
    ICacheService cacheService,
    IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository,
    IMediator mediator)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
    _mediator = mediator;
  }

  public async Task Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    Configuration? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration is null)
    {
      UserId userId = UserId.NewId(realmId: null);
      ActorId actorId = new(userId.Value);

      //Secret secret = _secretHelper.Generate();
      configuration = Configuration.Initialize(actorId/*, secret*/);

      Locale locale = new(command.DefaultLocale);
      Language language = new(locale, isDefault: true, actorId);

      UniqueName uniqueName = new(configuration.UniqueNameSettings, command.UniqueName);
      User user = new(uniqueName, password: null, actorId, userId) // TODO(fpion): password
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
