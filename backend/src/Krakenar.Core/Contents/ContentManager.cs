using Krakenar.Core.Contents.Events;

namespace Krakenar.Core.Contents;

public interface IContentManager
{
  Task SaveAsync(ContentType contentType, CancellationToken cancellationToken = default);
}

public class ContentManager : IContentManager
{
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public ContentManager(IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
  }

  public virtual async Task SaveAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = contentType.Changes.Any(change => change is ContentTypeCreated || change is ContentTypeUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      ContentTypeId? conflictId = await ContentTypeQuerier.FindIdAsync(contentType.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(contentType.Id))
      {
        throw new UniqueNameAlreadyUsedException(contentType, conflictId.Value);
      }
    }

    await ContentTypeRepository.SaveAsync(contentType, cancellationToken);
  }
}
