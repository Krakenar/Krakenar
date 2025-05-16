using FluentValidation;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Realms;

namespace Krakenar.Core.Contents;

public interface IContentManager
{
  Task<ContentType> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken = default);
  Task SaveAsync(Content content, CancellationToken cancellationToken = default);
  Task SaveAsync(ContentType contentType, CancellationToken cancellationToken = default);
}

public class ContentManager : IContentManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public ContentManager(IApplicationContext applicationContext, IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
  }

  public virtual async Task<ContentType> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken)
  {
    RealmId? realmId = ApplicationContext.RealmId;
    ContentType? contentType = null;

    if (Guid.TryParse(idOrUniqueName, out Guid entityId))
    {
      ContentTypeId contentTypeId = new(entityId, realmId);
      contentType = await ContentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    }

    if (contentType is null)
    {
      try
      {
        Identifier uniqueName = new(idOrUniqueName);
        ContentTypeId? contentTypeId = await ContentTypeQuerier.FindIdAsync(uniqueName, cancellationToken);
        if (contentTypeId.HasValue)
        {
          contentType = await ContentTypeRepository.LoadAsync(contentTypeId.Value, cancellationToken);
        }
      }
      catch (ValidationException)
      {
      }
    }

    return contentType ?? throw new ContentTypeNotFoundException(realmId, idOrUniqueName, propertyName);
  }

  public virtual Task SaveAsync(Content content, CancellationToken cancellationToken)
  {
    return Task.CompletedTask; // TODO(fpion): implement
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
