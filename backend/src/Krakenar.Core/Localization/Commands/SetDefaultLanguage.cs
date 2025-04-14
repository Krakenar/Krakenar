using Logitar.EventSourcing;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

public record SetDefaultLanguage(Guid Id) : ICommand<LanguageDto?>;

public class SetDefaultLanguageHandler : ICommandHandler<SetDefaultLanguage, LanguageDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual ILanguageRepository LanguageRepository { get; }

  public SetDefaultLanguageHandler(IApplicationContext applicationContext, ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    ApplicationContext = applicationContext;
    LanguageQuerier = languageQuerier;
    LanguageRepository = languageRepository;
  }

  public virtual async Task<LanguageDto?> HandleAsync(SetDefaultLanguage command, CancellationToken cancellationToken)
  {
    LanguageId languageId = new(command.Id, ApplicationContext.RealmId);
    Language? language = await LanguageRepository.LoadAsync(languageId, cancellationToken);
    if (language is null)
    {
      return null;
    }
    else if (!language.IsDefault)
    {
      LanguageId defaultId = await LanguageQuerier.FindDefaultIdAsync(cancellationToken);
      Language @default = await LanguageRepository.LoadAsync(defaultId, cancellationToken)
        ?? throw new InvalidOperationException($"The default language 'Id={defaultId}' was not loaded.");

      ActorId? actorId = ApplicationContext.ActorId;
      @default.SetDefault(isDefault: false, actorId);
      language.SetDefault(isDefault: true, actorId);

      await LanguageRepository.SaveAsync([@default, language], cancellationToken);
    }

    return await LanguageQuerier.ReadAsync(language, cancellationToken);
  }
}
