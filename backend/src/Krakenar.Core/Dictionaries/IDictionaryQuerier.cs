using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Search;
using Krakenar.Core.Localization;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries;

public interface IDictionaryQuerier
{
  Task<DictionaryId?> FindIdAsync(LanguageId languageId, CancellationToken cancellationToken = default);

  Task<DictionaryDto> ReadAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
  Task<DictionaryDto?> ReadAsync(DictionaryId id, CancellationToken cancellationToken = default);
  Task<DictionaryDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<DictionaryDto?> ReadByLanguageAsync(Guid languageId, CancellationToken cancellationToken = default);

  Task<SearchResults<DictionaryDto>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken = default);
}
