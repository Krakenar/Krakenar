using FluentValidation;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents;

public interface IContentManager
{
  Task<ContentType> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken = default);
  Task SaveAsync(Content content, ContentType? contentType = null, CancellationToken cancellationToken = default);
  Task SaveAsync(ContentType contentType, CancellationToken cancellationToken = default);
}

public class ContentManager : IContentManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IContentRepository ContentRepository { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public ContentManager(
    IApplicationContext applicationContext,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentQuerier = contentQuerier;
    ContentRepository = contentRepository;
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

  public virtual async Task SaveAsync(Content content, ContentType? contentType, CancellationToken cancellationToken)
  {
    bool hasInvariantChanged = false;
    HashSet<LanguageId> languageIds = [];
    foreach (IEvent @event in content.Changes)
    {
      if (@event is ContentCreated)
      {
        hasInvariantChanged = true;
      }
      else if (@event is ContentLocaleChanged changed)
      {
        if (changed.LanguageId.HasValue)
        {
          languageIds.Add(changed.LanguageId.Value);
        }
        else
        {
          hasInvariantChanged = true;
        }
      }
    }

    if (hasInvariantChanged)
    {
      ContentLocale invariant = content.Invariant;
      UniqueName uniqueName = invariant.UniqueName;
      ContentId? conflictId = await ContentQuerier.FindIdAsync(content.ContentTypeId, languageId: null, uniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(content.Id))
      {
        throw new ContentUniqueNameAlreadyUsedException(content, languageId: null, conflictId.Value, uniqueName);
      }
    }

    foreach (LanguageId languageId in languageIds)
    {
      ContentLocale locale = content.FindLocale(languageId);
      UniqueName uniqueName = locale.UniqueName;
      ContentId? conflictId = await ContentQuerier.FindIdAsync(content.ContentTypeId, languageId, uniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(content.Id))
      {
        throw new ContentUniqueNameAlreadyUsedException(content, languageId, conflictId.Value, uniqueName);
      }
    }

    await ContentRepository.SaveAsync(content, cancellationToken);
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
