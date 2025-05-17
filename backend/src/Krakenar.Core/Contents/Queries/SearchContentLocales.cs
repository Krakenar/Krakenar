using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using ContentLocaleDto = Krakenar.Contracts.Contents.ContentLocale;

namespace Krakenar.Core.Contents.Queries;

public record SearchContentLocales(SearchContentLocalesPayload Payload) : IQuery<SearchResults<ContentLocaleDto>>;

public class SearchContentLocalesHandler : IQueryHandler<SearchContentLocales, SearchResults<ContentLocaleDto>>
{
  protected virtual IContentLocaleQuerier ContentLocaleQuerier { get; }

  public SearchContentLocalesHandler(IContentLocaleQuerier contentLocaleQuerier)
  {
    ContentLocaleQuerier = contentLocaleQuerier;
  }

  public virtual async Task<SearchResults<ContentLocaleDto>> HandleAsync(SearchContentLocales query, CancellationToken cancellationToken)
  {
    return await ContentLocaleQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
