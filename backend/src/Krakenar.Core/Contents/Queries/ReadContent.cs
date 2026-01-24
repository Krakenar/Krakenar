using Logitar.CQRS;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents.Queries;

public record ReadContent(Guid Id) : IQuery<ContentDto?>;

public class ReadContentHandler : IQueryHandler<ReadContent, ContentDto?>
{
  protected virtual IContentQuerier ContentQuerier { get; }

  public ReadContentHandler(IContentQuerier contentQuerier)
  {
    ContentQuerier = contentQuerier;
  }

  public virtual async Task<ContentDto?> HandleAsync(ReadContent query, CancellationToken cancellationToken)
  {
    return await ContentQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
