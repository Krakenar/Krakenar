using FluentValidation;
using Krakenar.Core.Localization.Events;
using Krakenar.Core.Realms;

namespace Krakenar.Core.Localization;

public interface ILanguageManager
{
  Task<Language> FindAsync(string idOrLocaleCode, string propertyName, CancellationToken cancellationToken = default);
  Task SaveAsync(Language language, CancellationToken cancellationToken = default);
}

public class LanguageManager : ILanguageManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual ILanguageRepository LanguageRepository { get; }

  public LanguageManager(IApplicationContext applicationContext, ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    ApplicationContext = applicationContext;
    LanguageQuerier = languageQuerier;
    LanguageRepository = languageRepository;
  }

  public virtual async Task<Language> FindAsync(string idOrLocaleCode, string propertyName, CancellationToken cancellationToken)
  {
    Language? language = null;
    RealmId? realmId = ApplicationContext.RealmId;

    if (Guid.TryParse(idOrLocaleCode, out Guid entityId))
    {
      LanguageId languageId = new(entityId, realmId);
      language = await LanguageRepository.LoadAsync(languageId, cancellationToken);
    }

    if (language is null)
    {
      try
      {
        Locale locale = new(idOrLocaleCode.Trim());
        LanguageId? languageId = await LanguageQuerier.FindIdAsync(locale, cancellationToken);
        if (languageId.HasValue)
        {
          language = await LanguageRepository.LoadAsync(languageId.Value, cancellationToken);
        }
      }
      catch (ValidationException)
      {
      }
    }

    return language ?? throw new LanguageNotFoundException(realmId, idOrLocaleCode, propertyName);
  }

  public virtual async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    bool hasLocaleChanged = language.Changes.Any(change => change is LanguageCreated || change is LanguageLocaleChanged);
    if (hasLocaleChanged)
    {
      LanguageId? conflictId = await LanguageQuerier.FindIdAsync(language.Locale, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(language.Id))
      {
        throw new LocaleAlreadyUsedException(language, conflictId.Value);
      }
    }

    await LanguageRepository.SaveAsync(language, cancellationToken);
  }
}
