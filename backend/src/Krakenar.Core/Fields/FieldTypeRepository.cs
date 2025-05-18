using Logitar.EventSourcing;

namespace Krakenar.Core.Fields;

public interface IFieldTypeRepository
{
  Task<FieldType?> LoadAsync(FieldTypeId id, CancellationToken cancellationToken = default);
  Task<FieldType?> LoadAsync(FieldTypeId id, long? version, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<FieldType>> LoadAsync(IEnumerable<FieldTypeId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<FieldType> fieldTypes, CancellationToken cancellationToken = default);
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
  public virtual async Task<IReadOnlyCollection<FieldType>> LoadAsync(IEnumerable<FieldTypeId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync<FieldType>(ids.Select(id => id.StreamId), cancellationToken);
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
