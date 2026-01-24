using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Logitar.CQRS;

namespace Krakenar.Core.Contents.Queries;

public record SearchPublishedContents(SearchPublishedContentsPayload Payload) : IQuery<SearchResults<PublishedContentLocale>>;

public class SearchPublishedContentsHandler : IQueryHandler<SearchPublishedContents, SearchResults<PublishedContentLocale>>
{
  protected virtual IPublishedContentQuerier PublishedContentQuerier { get; }

  public SearchPublishedContentsHandler(IPublishedContentQuerier publishedContentQuerier)
  {
    PublishedContentQuerier = publishedContentQuerier;
  }

  public virtual async Task<SearchResults<PublishedContentLocale>> HandleAsync(SearchPublishedContents query, CancellationToken cancellationToken)
  {
    return await PublishedContentQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
