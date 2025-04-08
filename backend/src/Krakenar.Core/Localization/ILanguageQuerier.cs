namespace Krakenar.Core.Localization;

public interface ILanguageQuerier
{
  Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken = default);
}
