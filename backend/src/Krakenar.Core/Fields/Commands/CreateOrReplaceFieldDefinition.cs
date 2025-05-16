using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields.Validators;
using ContentType = Krakenar.Core.Contents.ContentType;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Fields.Commands;

public record CreateOrReplaceFieldDefinition(Guid ContentTypeId, CreateOrReplaceFieldDefinitionPayload Payload, Guid? FieldId) : ICommand<ContentTypeDto?>;

/// <exception cref="FieldUniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceFieldDefinitionHandler : ICommandHandler<CreateOrReplaceFieldDefinition, ContentTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }
  protected virtual IFieldManager FieldManager { get; }

  public CreateOrReplaceFieldDefinitionHandler(
    IApplicationContext applicationContext,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository,
    IFieldManager fieldManager)
  {
    ApplicationContext = applicationContext;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
    FieldManager = fieldManager;
  }

  public virtual async Task<ContentTypeDto?> HandleAsync(CreateOrReplaceFieldDefinition command, CancellationToken cancellationToken)
  {
    ContentTypeId contentTypeId = new(command.ContentTypeId, ApplicationContext.RealmId);
    ContentType? contentType = await ContentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType is null)
    {
      return null;
    }

    CreateOrReplaceFieldDefinitionPayload payload = command.Payload;
    new CreateOrReplaceFieldDefinitionValidator(contentType.IsInvariant).ValidateAndThrow(payload);

    FieldTypeId fieldTypeId;
    FieldDefinition? existingField = command.FieldId.HasValue ? contentType.TryGetField(command.FieldId.Value) : null;
    if (existingField is null)
    {
      FieldType fieldType = await FieldManager.FindAsync(payload.FieldType, nameof(payload.FieldType), cancellationToken);
      fieldTypeId = fieldType.Id;
    }
    else
    {
      fieldTypeId = existingField.FieldTypeId;
    }

    FieldDefinition fieldDefinition = new(
      command.FieldId ?? Guid.NewGuid(),
      fieldTypeId,
      payload.IsInvariant,
      payload.IsRequired,
      payload.IsIndexed,
      payload.IsRequired,
      new Identifier(payload.UniqueName),
      DisplayName.TryCreate(payload.DisplayName),
      Description.TryCreate(payload.Description),
      Placeholder.TryCreate(payload.Placeholder));
    contentType.SetField(fieldDefinition, ApplicationContext.ActorId);

    await ContentTypeRepository.SaveAsync(contentType, cancellationToken);

    return await ContentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}
