using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Fields;

public interface IFieldTypeService
{
  Task<CreateOrReplaceFieldTypeResult> CreateOrReplaceAsync(CreateOrReplaceFieldTypePayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<FieldType?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<FieldType?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<FieldType>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken = default);
  Task<FieldType?> UpdateAsync(Guid id, UpdateFieldTypePayload payload, CancellationToken cancellationToken = default);
}
