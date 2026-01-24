using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries.Queries;

public record SearchDictionaries(SearchDictionariesPayload Payload) : IQuery<SearchResults<DictionaryDto>>;

public class SearchDictionariesHandler : IQueryHandler<SearchDictionaries, SearchResults<DictionaryDto>>
{
  protected virtual IDictionaryQuerier DictionaryQuerier { get; }

  public SearchDictionariesHandler(IDictionaryQuerier dictionaryQuerier)
  {
    DictionaryQuerier = dictionaryQuerier;
  }

  public virtual async Task<SearchResults<DictionaryDto>> HandleAsync(SearchDictionaries query, CancellationToken cancellationToken)
  {
    return await DictionaryQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
