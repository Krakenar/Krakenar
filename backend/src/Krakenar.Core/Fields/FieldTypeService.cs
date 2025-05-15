using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using Krakenar.Core.Fields.Commands;
using Krakenar.Core.Fields.Queries;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.Core.Fields;

public class FieldTypeService : IFieldTypeService
{
  protected virtual ICommandHandler<CreateOrReplaceFieldType, CreateOrReplaceFieldTypeResult> CreateOrReplaceFieldType { get; }
  protected virtual ICommandHandler<DeleteFieldType, FieldTypeDto?> DeleteFieldType { get; }
  protected virtual IQueryHandler<ReadFieldType, FieldTypeDto?> ReadFieldType { get; }
  protected virtual IQueryHandler<SearchFieldTypes, SearchResults<FieldTypeDto>> SearchFieldTypes { get; }
  protected virtual ICommandHandler<UpdateFieldType, FieldTypeDto?> UpdateFieldType { get; }

  public FieldTypeService(
    ICommandHandler<CreateOrReplaceFieldType, CreateOrReplaceFieldTypeResult> createOrReplaceFieldType,
    ICommandHandler<DeleteFieldType, FieldTypeDto?> deleteFieldType,
    IQueryHandler<ReadFieldType, FieldTypeDto?> readFieldType,
    IQueryHandler<SearchFieldTypes, SearchResults<FieldTypeDto>> searchFieldTypes,
    ICommandHandler<UpdateFieldType, FieldTypeDto?> updateFieldType)
  {
    CreateOrReplaceFieldType = createOrReplaceFieldType;
    DeleteFieldType = deleteFieldType;
    ReadFieldType = readFieldType;
    SearchFieldTypes = searchFieldTypes;
    UpdateFieldType = updateFieldType;
  }

  public virtual async Task<CreateOrReplaceFieldTypeResult> CreateOrReplaceAsync(CreateOrReplaceFieldTypePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldType command = new(id, payload, version);
    return await CreateOrReplaceFieldType.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<FieldTypeDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteFieldType command = new(id);
    return await DeleteFieldType.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<FieldTypeDto?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadFieldType query = new(id, uniqueName);
    return await ReadFieldType.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<FieldTypeDto>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken)
  {
    SearchFieldTypes query = new(payload);
    return await SearchFieldTypes.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<FieldTypeDto?> UpdateAsync(Guid id, UpdateFieldTypePayload payload, CancellationToken cancellationToken)
  {
    UpdateFieldType command = new(id, payload);
    return await UpdateFieldType.HandleAsync(command, cancellationToken);
  }
}
