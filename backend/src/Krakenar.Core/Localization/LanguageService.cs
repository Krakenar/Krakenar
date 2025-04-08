using Krakenar.Core.Localization.Events;

namespace Krakenar.Core.Localization;

public interface ILanguageService
{
  Task SaveAsync(Language language, CancellationToken cancellationToken = default);
}

public class LanguageService : ILanguageService
{
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual ILanguageRepository LanguageRepository { get; }

  public LanguageService(ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    LanguageQuerier = languageQuerier;
    LanguageRepository = languageRepository;
  }

  public virtual async Task SaveAsync(Language language, CancellationToken cancellationToken)
  {
    bool hasLocaleChanged = language.Changes.Any(change => change is LanguageCreated);
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
