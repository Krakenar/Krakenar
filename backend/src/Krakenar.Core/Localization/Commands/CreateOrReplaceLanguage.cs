using FluentValidation;
using Krakenar.Contracts.Localization;
using Krakenar.Core.Localization.Validators;
using Logitar.EventSourcing;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

public record CreateOrReplaceLanguageResult(LanguageDto? Language = null, bool Created = false);

public record CreateOrReplaceLanguage(Guid? Id, CreateOrReplaceLanguagePayload Payload, long? Version) : ICommand<CreateOrReplaceLanguageResult>;

/// <exception cref="LocaleAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceLanguageHandler : ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual ILanguageRepository LanguageRepository { get; }
  protected virtual ILanguageService LanguageService { get; }

  public CreateOrReplaceLanguageHandler(
    IApplicationContext applicationContext,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository,
    ILanguageService languageService)
  {
    ApplicationContext = applicationContext;
    LanguageQuerier = languageQuerier;
    LanguageRepository = languageRepository;
    LanguageService = languageService;
  }

  public virtual async Task<CreateOrReplaceLanguageResult> HandleAsync(CreateOrReplaceLanguage command, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguagePayload payload = command.Payload;
    new CreateOrReplaceLanguageValidator().ValidateAndThrow(payload);

    LanguageId languageId = LanguageId.NewId(ApplicationContext.RealmId);
    Language? language = null;
    if (command.Id.HasValue)
    {
      languageId = new(command.Id.Value, languageId.RealmId);
      language = await LanguageRepository.LoadAsync(languageId, cancellationToken);
    }

    Locale locale = new(payload.Locale);
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    if (language is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceLanguageResult();
      }

      language = new Language(locale, isDefault: false, actorId, languageId);
      created = true;
    }

    Language reference = (command.Version.HasValue
      ? await LanguageRepository.LoadAsync(languageId, command.Version, cancellationToken)
      : null) ?? language;

    if (reference.Locale != locale)
    {
      language.SetLocale(locale, actorId);
    }

    await LanguageService.SaveAsync(language, cancellationToken);

    LanguageDto dto = await LanguageQuerier.ReadAsync(language, cancellationToken);
    return new CreateOrReplaceLanguageResult(dto, created);
  }
}
