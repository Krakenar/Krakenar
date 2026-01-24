using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields.Validators;
using Logitar.CQRS;
using ContentType = Krakenar.Core.Contents.ContentType;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Fields.Commands;

public record UpdateFieldDefinition(Guid ContentTypeId, Guid FieldId, UpdateFieldDefinitionPayload Payload) : ICommand<ContentTypeDto?>;

/// <exception cref="FieldUniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateFieldDefinitionHandler : ICommandHandler<UpdateFieldDefinition, ContentTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public UpdateFieldDefinitionHandler(IApplicationContext applicationContext, IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
  }

  public virtual async Task<ContentTypeDto?> HandleAsync(UpdateFieldDefinition command, CancellationToken cancellationToken)
  {
    ContentTypeId contentTypeId = new(command.ContentTypeId, ApplicationContext.RealmId);
    ContentType? contentType = await ContentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType is null)
    {
      return null;
    }

    UpdateFieldDefinitionPayload payload = command.Payload;
    new UpdateFieldDefinitionValidator(contentType.IsInvariant).ValidateAndThrow(payload);

    FieldDefinition? fieldDefinition = contentType.TryGetField(command.FieldId);
    if (fieldDefinition is null)
    {
      return null;
    }

    Identifier uniqueName = Identifier.TryCreate(payload.UniqueName) ?? fieldDefinition.UniqueName;
    DisplayName? displayName = payload.DisplayName is null ? fieldDefinition.DisplayName : DisplayName.TryCreate(payload.DisplayName.Value);
    Description? description = payload.Description is null ? fieldDefinition.Description : Description.TryCreate(payload.Description.Value);
    Placeholder? placeholder = payload.Placeholder is null ? fieldDefinition.Placeholder : Placeholder.TryCreate(payload.Placeholder.Value);
    fieldDefinition = new(
      fieldDefinition.Id,
      fieldDefinition.FieldTypeId,
      payload.IsInvariant ?? fieldDefinition.IsInvariant,
      payload.IsRequired ?? fieldDefinition.IsRequired,
      payload.IsIndexed ?? fieldDefinition.IsIndexed,
      payload.IsUnique ?? fieldDefinition.IsUnique,
      uniqueName,
      displayName,
      description,
      placeholder);
    contentType.SetField(fieldDefinition, ApplicationContext.ActorId);

    await ContentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await ContentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}
