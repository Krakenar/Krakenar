using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core.Contents.Commands;
using Krakenar.Core.Contents.Queries;
using Logitar.CQRS;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Contents;

public class ContentTypeService : IContentTypeService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public ContentTypeService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<CreateOrReplaceContentTypeResult> CreateOrReplaceAsync(CreateOrReplaceContentTypePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentType command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteContentType command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadContentType query = new(id, uniqueName);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<ContentTypeDto>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken)
  {
    SearchContentTypes query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> UpdateAsync(Guid id, UpdateContentTypePayload payload, CancellationToken cancellationToken)
  {
    UpdateContentType command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
