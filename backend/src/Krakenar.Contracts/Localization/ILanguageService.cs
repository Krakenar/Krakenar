using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Localization;

public interface ILanguageService
{
  Task<CreateOrReplaceLanguageResult> CreateOrReplaceAsync(CreateOrReplaceLanguagePayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<Language?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Language?> ReadAsync(Guid? id = null, string? locale = null, bool isDefault = false, CancellationToken cancellationToken = default);
  Task<SearchResults<Language>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken = default);
  Task<Language?> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Language?> UpdateAsync(Guid id, UpdateLanguagePayload payload, CancellationToken cancellationToken = default);
}
