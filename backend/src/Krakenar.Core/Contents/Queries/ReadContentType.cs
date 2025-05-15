using Krakenar.Contracts;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Contents.Queries;

public record ReadContentType(Guid? Id, string? UniqueName) : IQuery<ContentTypeDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadContentTypeHandler : IQueryHandler<ReadContentType, ContentTypeDto?>
{
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }

  public ReadContentTypeHandler(IContentTypeQuerier contentTypeQuerier)
  {
    ContentTypeQuerier = contentTypeQuerier;
  }

  public virtual async Task<ContentTypeDto?> HandleAsync(ReadContentType query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, ContentTypeDto> contentTypes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      ContentTypeDto? contentType = await ContentTypeQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (contentType is not null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      ContentTypeDto? contentType = await ContentTypeQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (contentType is not null)
      {
        contentTypes[contentType.Id] = contentType;
      }
    }

    if (contentTypes.Count > 1)
    {
      throw TooManyResultsException<ContentTypeDto>.ExpectedSingle(contentTypes.Count);
    }

    return contentTypes.SingleOrDefault().Value;
  }
}
