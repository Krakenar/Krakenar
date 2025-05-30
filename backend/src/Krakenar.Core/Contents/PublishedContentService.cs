using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core.Contents.Queries;

namespace Krakenar.Core.Contents;

public class PublishedContentService : IPublishedContentService
{
  protected virtual IQueryBus QueryBus { get; }

  public PublishedContentService(IQueryBus queryBus)
  {
    QueryBus = queryBus;
  }

  public virtual async Task<PublishedContent?> ReadAsync(int? id, Guid? uid, PublishedContentKey? key, CancellationToken cancellationToken)
  {
    ReadPublishedContent query = new(id, uid, key);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<PublishedContentLocale>> SearchAsync(SearchPublishedContentsPayload payload, CancellationToken cancellationToken)
  {
    SearchPublishedContents query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }
}
