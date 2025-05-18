using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core.Contents.Queries;

namespace Krakenar.Core.Contents;

public class PublishedContentService : IPublishedContentService
{
  protected virtual IQueryHandler<ReadPublishedContent, PublishedContent?> ReadPublishedContent { get; }
  protected virtual IQueryHandler<SearchPublishedContents, SearchResults<PublishedContentLocale>> SearchPublishedContents { get; }

  public PublishedContentService(
    IQueryHandler<ReadPublishedContent, PublishedContent?> readPublishedContent,
    IQueryHandler<SearchPublishedContents, SearchResults<PublishedContentLocale>> searchPublishedContents)
  {
    ReadPublishedContent = readPublishedContent;
    SearchPublishedContents = searchPublishedContents;
  }

  public virtual async Task<PublishedContent?> ReadAsync(int? id, Guid? uid, PublishedContentKey? key, CancellationToken cancellationToken)
  {
    ReadPublishedContent query = new(id, uid, key);
    return await ReadPublishedContent.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<PublishedContentLocale>> SearchAsync(SearchPublishedContentsPayload payload, CancellationToken cancellationToken)
  {
    SearchPublishedContents query = new(payload);
    return await SearchPublishedContents.HandleAsync(query, cancellationToken);
  }
}
