using Krakenar.Contracts.Contents;

namespace Krakenar.Contracts.Fields;

public interface IFieldDefinitionService
{
  Task<ContentType?> CreateOrReplaceAsync(Guid contentTypeId, CreateOrReplaceFieldDefinitionPayload payload, Guid? fieldId = null, long? version = null, CancellationToken cancellationToken = default);
  Task<ContentType?> DeleteAsync(Guid contentTypeId, Guid fieldId, CancellationToken cancellationToken = default);
  Task<ContentType?> UpdateAsync(Guid contentTypeId, Guid fieldId, UpdateFieldDefinitionPayload payload, CancellationToken cancellationToken = default);
}
