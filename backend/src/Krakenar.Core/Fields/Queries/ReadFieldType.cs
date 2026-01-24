using Krakenar.Contracts;
using Logitar.CQRS;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.Core.Fields.Queries;

public record ReadFieldType(Guid? Id, string? UniqueName) : IQuery<FieldTypeDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadFieldTypeHandler : IQueryHandler<ReadFieldType, FieldTypeDto?>
{
  protected virtual IFieldTypeQuerier FieldTypeQuerier { get; }

  public ReadFieldTypeHandler(IFieldTypeQuerier fieldTypeQuerier)
  {
    FieldTypeQuerier = fieldTypeQuerier;
  }

  public virtual async Task<FieldTypeDto?> HandleAsync(ReadFieldType query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, FieldTypeDto> fieldTypes = new(capacity: 2);

    if (query.Id.HasValue)
    {
      FieldTypeDto? fieldType = await FieldTypeQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (fieldType is not null)
      {
        fieldTypes[fieldType.Id] = fieldType;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      FieldTypeDto? fieldType = await FieldTypeQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (fieldType is not null)
      {
        fieldTypes[fieldType.Id] = fieldType;
      }
    }

    if (fieldTypes.Count > 1)
    {
      throw TooManyResultsException<FieldTypeDto>.ExpectedSingle(fieldTypes.Count);
    }

    return fieldTypes.SingleOrDefault().Value;
  }
}
