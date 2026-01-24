using FluentValidation;
using Krakenar.Contracts.Localization;
using Krakenar.Core.Localization.Validators;
using Logitar.CQRS;
using Logitar.EventSourcing;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

public record CreateOrReplaceLanguage(Guid? Id, CreateOrReplaceLanguagePayload Payload, long? Version) : ICommand<CreateOrReplaceLanguageResult>;

/// <exception cref="LocaleAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceLanguageHandler : ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ILanguageManager LanguageManager { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual ILanguageRepository LanguageRepository { get; }

  public CreateOrReplaceLanguageHandler(
    IApplicationContext applicationContext,
    ILanguageManager languageManager,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository)
  {
    ApplicationContext = applicationContext;
    LanguageManager = languageManager;
    LanguageQuerier = languageQuerier;
    LanguageRepository = languageRepository;
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

    await LanguageManager.SaveAsync(language, cancellationToken);

    LanguageDto dto = await LanguageQuerier.ReadAsync(language, cancellationToken);
    return new CreateOrReplaceLanguageResult(dto, created);
  }
}
