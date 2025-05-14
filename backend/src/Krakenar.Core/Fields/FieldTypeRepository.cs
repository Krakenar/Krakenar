using Logitar.EventSourcing;

namespace Krakenar.Core.Fields;

public interface IFieldTypeRepository
{
  Task<FieldType?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken);
  Task<FieldType?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken);

  Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken);
  Task SaveAsync(IEnumerable<FieldType> fieldTypes, CancellationToken cancellationToken);
}

public class FieldTypeRepository : Repository, IFieldTypeRepository
{
  public FieldTypeRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<FieldType?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<FieldType?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<FieldType>(id.StreamId, version, cancellationToken);
  }

  public virtual async Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(fieldType, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<FieldType> fieldTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(fieldTypes, cancellationToken);
  }
}
