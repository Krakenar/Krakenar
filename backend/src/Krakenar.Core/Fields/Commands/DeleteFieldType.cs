using Krakenar.Core.Contents;
using Logitar.EventSourcing;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.Core.Fields.Commands;

public record DeleteFieldType(Guid Id) : ICommand<FieldTypeDto?>;

public class DeleteFieldTypeHandler : ICommandHandler<DeleteFieldType, FieldTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }
  protected virtual IFieldTypeQuerier FieldTypeQuerier { get; }
  protected virtual IFieldTypeRepository FieldTypeRepository { get; }

  public DeleteFieldTypeHandler(
    IApplicationContext applicationContext,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository,
    IFieldTypeQuerier fieldTypeQuerier,
    IFieldTypeRepository fieldTypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
    FieldTypeQuerier = fieldTypeQuerier;
    FieldTypeRepository = fieldTypeRepository;
  }

  public virtual async Task<FieldTypeDto?> HandleAsync(DeleteFieldType command, CancellationToken cancellationToken)
  {
    FieldTypeId fieldTypeId = new(command.Id, ApplicationContext.RealmId);
    FieldType? fieldType = await FieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    if (fieldType is null)
    {
      return null;
    }
    FieldTypeDto dto = await FieldTypeQuerier.ReadAsync(fieldType, cancellationToken);

    ActorId? actorId = ApplicationContext.ActorId;

    IReadOnlyCollection<ContentTypeId> contentTypeIds = await ContentTypeQuerier.FindIdsAsync(fieldType.Id, cancellationToken);
    if (contentTypeIds.Count > 0)
    {
      IReadOnlyCollection<ContentType> contentTypes = await ContentTypeRepository.LoadAsync(contentTypeIds, cancellationToken);
      foreach (ContentType contentType in contentTypes)
      {
        FieldDefinition[] fields = [.. contentType.Fields];
        foreach (FieldDefinition field in fields)
        {
          if (field.FieldTypeId == fieldType.Id)
          {
            contentType.RemoveField(field.Id, actorId);
          }
        }
      }
      await ContentTypeRepository.SaveAsync(contentTypes, cancellationToken);
    }

    fieldType.Delete(actorId);
    await FieldTypeRepository.SaveAsync(fieldType, cancellationToken);

    return dto;
  }
}
