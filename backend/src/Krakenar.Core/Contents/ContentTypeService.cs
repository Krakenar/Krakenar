using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core.Contents.Commands;
using Krakenar.Core.Contents.Queries;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Contents;

public class ContentTypeService : IContentTypeService
{
  protected virtual ICommandHandler<CreateOrReplaceContentType, CreateOrReplaceContentTypeResult> CreateOrReplaceContentType { get; }
  protected virtual ICommandHandler<DeleteContentType, ContentTypeDto?> DeleteContentType { get; }
  protected virtual IQueryHandler<ReadContentType, ContentTypeDto?> ReadContentType { get; }
  protected virtual IQueryHandler<SearchContentTypes, SearchResults<ContentTypeDto>> SearchContentTypes { get; }
  protected virtual ICommandHandler<UpdateContentType, ContentTypeDto?> UpdateContentType { get; }

  public ContentTypeService(
    ICommandHandler<CreateOrReplaceContentType, CreateOrReplaceContentTypeResult> createOrReplaceContentType,
    ICommandHandler<DeleteContentType, ContentTypeDto?> deleteContentType,
    IQueryHandler<ReadContentType, ContentTypeDto?> readContentType,
    IQueryHandler<SearchContentTypes, SearchResults<ContentTypeDto>> searchContentTypes,
    ICommandHandler<UpdateContentType, ContentTypeDto?> updateContentType)
  {
    CreateOrReplaceContentType = createOrReplaceContentType;
    DeleteContentType = deleteContentType;
    ReadContentType = readContentType;
    SearchContentTypes = searchContentTypes;
    UpdateContentType = updateContentType;
  }

  public virtual async Task<CreateOrReplaceContentTypeResult> CreateOrReplaceAsync(CreateOrReplaceContentTypePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentType command = new(id, payload, version);
    return await CreateOrReplaceContentType.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteContentType command = new(id);
    return await DeleteContentType.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadContentType query = new(id, uniqueName);
    return await ReadContentType.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<ContentTypeDto>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken)
  {
    SearchContentTypes query = new(payload);
    return await SearchContentTypes.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> UpdateAsync(Guid id, UpdateContentTypePayload payload, CancellationToken cancellationToken)
  {
    UpdateContentType command = new(id, payload);
    return await UpdateContentType.HandleAsync(command, cancellationToken);
  }
}
