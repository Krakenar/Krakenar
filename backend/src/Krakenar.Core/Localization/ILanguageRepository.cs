namespace Krakenar.Core.Localization;

public interface ILanguageRepository
{
  Task SaveAsync(Language language, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Language> languages, CancellationToken cancellationToken = default);
}
