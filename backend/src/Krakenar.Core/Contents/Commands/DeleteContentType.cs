using Logitar.EventSourcing;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Contents.Commands;

public record DeleteContentType(Guid Id) : ICommand<ContentTypeDto?>;

public class DeleteContentTypeHandler : ICommandHandler<DeleteContentType, ContentTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentQuerier ContentQuerier { get; }
  protected virtual IContentRepository ContentRepository { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public DeleteContentTypeHandler(
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

  public virtual async Task<ContentTypeDto?> HandleAsync(DeleteContentType command, CancellationToken cancellationToken)
  {
    ContentTypeId contentTypeId = new(command.Id, ApplicationContext.RealmId);
    ContentType? contentType = await ContentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType is null)
    {
      return null;
    }
    ContentTypeDto dto = await ContentTypeQuerier.ReadAsync(contentType, cancellationToken);

    ActorId? actorId = ApplicationContext.ActorId;

    IReadOnlyCollection<ContentId> contentIds = await ContentQuerier.FindIdsAsync(contentType.Id, cancellationToken);
    if (contentIds.Count > 0)
    {
      IReadOnlyCollection<Content> contents = await ContentRepository.LoadAsync(contentIds, cancellationToken);
      foreach (Content content in contents)
      {
        content.Delete(actorId);
      }
      await ContentRepository.SaveAsync(contents, cancellationToken);
    }

    contentType.Delete(actorId);
    await ContentTypeRepository.SaveAsync(contentType, cancellationToken);

    return dto;
  }
}
