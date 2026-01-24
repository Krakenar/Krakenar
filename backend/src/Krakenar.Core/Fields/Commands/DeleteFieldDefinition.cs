using FluentValidation;
using Krakenar.Core.Contents;
using Logitar.CQRS;
using ContentType = Krakenar.Core.Contents.ContentType;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Fields.Commands;

public record DeleteFieldDefinition(Guid ContentTypeId, Guid FieldId) : ICommand<ContentTypeDto?>;

/// <exception cref="ValidationException"></exception>
public class DeleteFieldDefinitionHandler : ICommandHandler<DeleteFieldDefinition, ContentTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public DeleteFieldDefinitionHandler(IApplicationContext applicationContext, IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
  }

  public virtual async Task<ContentTypeDto?> HandleAsync(DeleteFieldDefinition command, CancellationToken cancellationToken)
  {
    ContentTypeId contentTypeId = new(command.ContentTypeId, ApplicationContext.RealmId);
    ContentType? contentType = await ContentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType is null || !contentType.RemoveField(command.FieldId, ApplicationContext.ActorId))
    {
      return null;
    }

    await ContentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await ContentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}
