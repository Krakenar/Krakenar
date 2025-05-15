using Krakenar.Contracts.Fields;
using Krakenar.Core.Fields.Commands;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Fields;

public class FieldDefinitionService : IFieldDefinitionService
{
  protected virtual ICommandHandler<CreateOrReplaceFieldDefinition, ContentTypeDto?> CreateOrReplaceFieldDefinition { get; }
  protected virtual ICommandHandler<DeleteFieldDefinition, ContentTypeDto?> DeleteFieldDefinition { get; }
  protected virtual ICommandHandler<UpdateFieldDefinition, ContentTypeDto?> UpdateFieldDefinition { get; }

  public FieldDefinitionService(
    ICommandHandler<CreateOrReplaceFieldDefinition, ContentTypeDto?> createOrReplaceFieldDefinition,
    ICommandHandler<DeleteFieldDefinition, ContentTypeDto?> deleteFieldDefinition,
    ICommandHandler<UpdateFieldDefinition, ContentTypeDto?> updateFieldDefinition)
  {
    CreateOrReplaceFieldDefinition = createOrReplaceFieldDefinition;
    DeleteFieldDefinition = deleteFieldDefinition;
    UpdateFieldDefinition = updateFieldDefinition;
  }

  public virtual async Task<ContentTypeDto?> CreateOrReplaceAsync(Guid contentTypeId, CreateOrReplaceFieldDefinitionPayload payload, Guid? fieldId, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldDefinition command = new(contentTypeId, payload, fieldId, version);
    return await CreateOrReplaceFieldDefinition.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> DeleteAsync(Guid contentTypeId, Guid fieldId, CancellationToken cancellationToken)
  {
    DeleteFieldDefinition command = new(contentTypeId, fieldId);
    return await DeleteFieldDefinition.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> UpdateAsync(Guid contentTypeId, Guid fieldId, UpdateFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    UpdateFieldDefinition command = new(contentTypeId, fieldId, payload);
    return await UpdateFieldDefinition.HandleAsync(command, cancellationToken);
  }
}
