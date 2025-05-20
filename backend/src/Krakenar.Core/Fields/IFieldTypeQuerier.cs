using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using Krakenar.Core.Contents;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.Core.Fields;

public interface IFieldTypeQuerier
{
  Task<FieldTypeId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<FieldTypeId>> FindIdsAsync(ContentTypeId contentTypeId, CancellationToken cancellationToken = default);

  Task<FieldTypeDto> ReadAsync(FieldType fieldType, CancellationToken cancellationToken = default);
  Task<FieldTypeDto?> ReadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldTypeDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<FieldTypeDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<FieldTypeDto>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken = default);
}
