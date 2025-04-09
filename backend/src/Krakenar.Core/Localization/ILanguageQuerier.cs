using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization;

public interface ILanguageQuerier
{
  Task<Locale> FindDefaultPlatformLocaleAsync(CancellationToken cancellationToken = default);
  Task<LanguageId?> FindIdAsync(Locale locale, CancellationToken cancellationToken = default);

  Task<LanguageDto> ReadAsync(Language language, CancellationToken cancellationToken = default);
  Task<LanguageDto?> ReadAsync(LanguageId id, CancellationToken cancellationToken = default);
  Task<LanguageDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<LanguageDto?> ReadAsync(string locale, CancellationToken cancellationToken = default);

  Task<LanguageDto> ReadDefaultAsync(CancellationToken cancellationToken = default);
}
