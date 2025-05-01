using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Dictionaries;

public interface IDictionaryService
{
  Task<CreateOrReplaceDictionaryResult> CreateOrReplaceAsync(CreateOrReplaceDictionaryPayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<Dictionary?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Dictionary?> ReadAsync(Guid? id = null, Guid? languageId = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Dictionary>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
  Task<Dictionary?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken = default);
}
