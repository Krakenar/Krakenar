using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Contents.Queries;

public record SearchContentTypes(SearchContentTypesPayload Payload) : IQuery<SearchResults<ContentTypeDto>>;

public class SearchContentTypesHandler : IQueryHandler<SearchContentTypes, SearchResults<ContentTypeDto>>
{
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }

  public SearchContentTypesHandler(IContentTypeQuerier contentTypeQuerier)
  {
    ContentTypeQuerier = contentTypeQuerier;
  }

  public virtual async Task<SearchResults<ContentTypeDto>> HandleAsync(SearchContentTypes query, CancellationToken cancellationToken)
  {
    return await ContentTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
