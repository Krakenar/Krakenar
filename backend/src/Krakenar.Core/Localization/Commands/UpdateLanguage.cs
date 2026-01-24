using FluentValidation;
using Krakenar.Contracts.Localization;
using Krakenar.Core.Localization.Validators;
using Logitar.CQRS;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

public record UpdateLanguage(Guid Id, UpdateLanguagePayload Payload) : ICommand<LanguageDto?>;

/// <exception cref="LocaleAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateLanguageHandler : ICommandHandler<UpdateLanguage, LanguageDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ILanguageManager LanguageManager { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual ILanguageRepository LanguageRepository { get; }

  public UpdateLanguageHandler(
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

  public virtual async Task<LanguageDto?> HandleAsync(UpdateLanguage command, CancellationToken cancellationToken)
  {
    UpdateLanguagePayload payload = command.Payload;
    new UpdateLanguageValidator().ValidateAndThrow(payload);

    LanguageId languageId = new(command.Id, ApplicationContext.RealmId);
    Language? language = await LanguageRepository.LoadAsync(languageId, cancellationToken);
    if (language is null)
    {
      return null;
    }

    if (!string.IsNullOrWhiteSpace(payload.Locale))
    {
      Locale locale = new(payload.Locale);
      language.SetLocale(locale, ApplicationContext.ActorId);

      await LanguageManager.SaveAsync(language, cancellationToken);
    }

    return await LanguageQuerier.ReadAsync(language, cancellationToken);
  }
}
