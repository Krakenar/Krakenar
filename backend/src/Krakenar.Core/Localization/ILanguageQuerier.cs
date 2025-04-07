namespace Krakenar.Core.Localization;

public interface ILanguageQuerier // TODO(fpion): implement
{
  Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken = default);
}
