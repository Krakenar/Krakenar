using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Queries;

public record SearchLanguages(SearchLanguagesPayload Payload) : IQuery<SearchResults<LanguageDto>>;

public class SearchLanguagesHandler : IQueryHandler<SearchLanguages, SearchResults<LanguageDto>>
{
  protected virtual ILanguageQuerier LanguageQuerier { get; }

  public SearchLanguagesHandler(ILanguageQuerier languageQuerier)
  {
    LanguageQuerier = languageQuerier;
  }

  public virtual async Task<SearchResults<LanguageDto>> HandleAsync(SearchLanguages query, CancellationToken cancellationToken)
  {
    return await LanguageQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
