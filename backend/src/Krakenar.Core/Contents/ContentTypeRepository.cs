using Logitar.EventSourcing;

namespace Krakenar.Core.Contents;

public interface IContentTypeRepository
{
  Task<ContentType?> LoadAsync(ContentTypeId id, CancellationToken cancellationToken = default);
  Task<ContentType?> LoadAsync(ContentTypeId id, long? version, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ContentType>> LoadAsync(IEnumerable<ContentTypeId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(ContentType contentType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<ContentType> contentTypes, CancellationToken cancellationToken = default);
}

public class ContentTypeRepository : Repository, IContentTypeRepository
{
  public ContentTypeRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<ContentType?> LoadAsync(ContentTypeId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<ContentType?> LoadAsync(ContentTypeId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<ContentType>(id.StreamId, version, cancellationToken);
  }
  public virtual async Task<IReadOnlyCollection<ContentType>> LoadAsync(IEnumerable<ContentTypeId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync<ContentType>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public virtual async Task SaveAsync(ContentType contentType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contentType, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<ContentType> contentTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contentTypes, cancellationToken);
  }
}
