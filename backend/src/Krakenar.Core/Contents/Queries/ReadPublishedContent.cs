using Krakenar.Contracts;
using Krakenar.Contracts.Contents;

namespace Krakenar.Core.Contents.Queries;

public record ReadPublishedContent(int? ContentId, Guid? ContentUid, PublishedContentKey? Key) : IQuery<PublishedContent?>;

internal class ReadPublishedContentHandler : IQueryHandler<ReadPublishedContent, PublishedContent?>
{
  protected virtual IPublishedContentQuerier PublishedContentQuerier { get; }

  public ReadPublishedContentHandler(IPublishedContentQuerier publishedContentQuerier)
  {
    PublishedContentQuerier = publishedContentQuerier;
  }

  public async Task<PublishedContent?> HandleAsync(ReadPublishedContent query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, PublishedContent> publishedContents = new(capacity: 3);

    if (query.ContentId.HasValue)
    {
      PublishedContent? publishedContent = await PublishedContentQuerier.ReadAsync(query.ContentId.Value, cancellationToken);
      if (publishedContent is not null)
      {
        publishedContents[publishedContent.Id] = publishedContent;
      }
    }

    if (query.ContentUid.HasValue)
    {
      PublishedContent? publishedContent = await PublishedContentQuerier.ReadAsync(query.ContentUid.Value, cancellationToken);
      if (publishedContent is not null)
      {
        publishedContents[publishedContent.Id] = publishedContent;
      }
    }

    if (query.Key is not null)
    {
      PublishedContent? publishedContent = await PublishedContentQuerier.ReadAsync(query.Key, cancellationToken);
      if (publishedContent is not null)
      {
        publishedContents[publishedContent.Id] = publishedContent;
      }
    }

    if (publishedContents.Count > 1)
    {
      throw TooManyResultsException<PublishedContent>.ExpectedSingle(publishedContents.Count);
    }

    return publishedContents.SingleOrDefault().Value;
  }
}
