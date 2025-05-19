using Logitar.EventSourcing;

namespace Krakenar.Core.Contents;

public interface IContentRepository
{
  Task<Content?> LoadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<Content?> LoadAsync(ContentId id, long? version, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Content>> LoadAsync(IEnumerable<ContentId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Content content, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Content> contents, CancellationToken cancellationToken = default);
}

public class ContentRepository : Repository, IContentRepository
{
  public ContentRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<Content?> LoadAsync(ContentId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<Content?> LoadAsync(ContentId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Content>(id.StreamId, version, cancellationToken);
  }
  public virtual async Task<IReadOnlyCollection<Content>> LoadAsync(IEnumerable<ContentId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync<Content>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public virtual async Task SaveAsync(Content content, CancellationToken cancellationToken)
  {
    await base.SaveAsync(content, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<Content> contents, CancellationToken cancellationToken)
  {
    await base.SaveAsync(contents, cancellationToken);
  }
}
