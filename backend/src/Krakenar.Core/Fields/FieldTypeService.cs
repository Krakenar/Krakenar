using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using Krakenar.Core.Fields.Commands;
using Krakenar.Core.Fields.Queries;
using Logitar.CQRS;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.Core.Fields;

public class FieldTypeService : IFieldTypeService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public FieldTypeService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<CreateOrReplaceFieldTypeResult> CreateOrReplaceAsync(CreateOrReplaceFieldTypePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldType command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<FieldTypeDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteFieldType command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<FieldTypeDto?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadFieldType query = new(id, uniqueName);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<FieldTypeDto>> SearchAsync(SearchFieldTypesPayload payload, CancellationToken cancellationToken)
  {
    SearchFieldTypes query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<FieldTypeDto?> UpdateAsync(Guid id, UpdateFieldTypePayload payload, CancellationToken cancellationToken)
  {
    UpdateFieldType command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
