using Krakenar.Contracts.Fields;
using Krakenar.Core.Fields.Commands;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Fields;

public class FieldDefinitionService : IFieldDefinitionService
{
  protected virtual ICommandBus CommandBus { get; }

  public FieldDefinitionService(ICommandBus commandBus)
  {
    CommandBus = commandBus;
  }

  public virtual async Task<ContentTypeDto?> CreateOrReplaceAsync(Guid contentTypeId, CreateOrReplaceFieldDefinitionPayload payload, Guid? fieldId, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldDefinition command = new(contentTypeId, payload, fieldId);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> DeleteAsync(Guid contentTypeId, Guid fieldId, CancellationToken cancellationToken)
  {
    DeleteFieldDefinition command = new(contentTypeId, fieldId);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentTypeDto?> UpdateAsync(Guid contentTypeId, Guid fieldId, UpdateFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    UpdateFieldDefinition command = new(contentTypeId, fieldId, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
