using Krakenar.Core.Fields;
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
  protected virtual IFieldTypeQuerier FieldTypeQuerier { get; }
  protected virtual IFieldTypeRepository FieldTypeRepository { get; }

  public DeleteContentTypeHandler(
    IApplicationContext applicationContext,
    IContentQuerier contentQuerier,
    IContentRepository contentRepository,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository,
    IFieldTypeQuerier fieldTypeQuerier,
    IFieldTypeRepository fieldTypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentQuerier = contentQuerier;
    ContentRepository = contentRepository;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
    FieldTypeQuerier = fieldTypeQuerier;
    FieldTypeRepository = fieldTypeRepository;
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

    HashSet<FieldTypeId> fieldTypeIds = (await FieldTypeQuerier.FindIdsAsync(contentType.Id, cancellationToken)).ToHashSet();
    if (fieldTypeIds.Count > 0)
    {
      IReadOnlyCollection<ContentTypeId> contentTypeIds = await ContentTypeQuerier.FindIdsAsync(fieldTypeIds, cancellationToken);
      IReadOnlyCollection<ContentType> contentTypes = await ContentTypeRepository.LoadAsync(contentTypeIds, cancellationToken);
      foreach (ContentType relatedContentType in contentTypes)
      {
        if (contentType.Id == relatedContentType.Id)
        {
          contentType = relatedContentType;
        }

        FieldDefinition[] fields = [.. relatedContentType.Fields];
        foreach (FieldDefinition field in fields)
        {
          if (fieldTypeIds.Contains(field.FieldTypeId))
          {
            relatedContentType.RemoveField(field.Id, actorId);
          }
        }
      }
      await ContentTypeRepository.SaveAsync(contentTypes, cancellationToken);

      IReadOnlyCollection<FieldType> fieldTypes = await FieldTypeRepository.LoadAsync(fieldTypeIds, cancellationToken);
      foreach (FieldType fieldType in fieldTypes)
      {
        fieldType.Delete(actorId);
      }
      await FieldTypeRepository.SaveAsync(fieldTypes, cancellationToken);
    }

    contentType.Delete(actorId);
    await ContentTypeRepository.SaveAsync(contentType, cancellationToken);

    return dto;
  }
}
