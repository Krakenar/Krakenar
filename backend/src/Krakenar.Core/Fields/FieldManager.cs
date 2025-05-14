using Krakenar.Core.Roles.Events;

namespace Krakenar.Core.Fields;

public interface IFieldManager
{
  Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken = default);
}

public class FieldManager : IFieldManager
{
  protected virtual IFieldTypeQuerier FieldTypeQuerier { get; }
  protected virtual IFieldTypeRepository FieldTypeRepository { get; }

  public FieldManager(IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository)
  {
    FieldTypeQuerier = fieldTypeQuerier;
    FieldTypeRepository = fieldTypeRepository;
  }

  public virtual async Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = fieldType.Changes.Any(change => change is RoleCreated || change is RoleUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      FieldTypeId? conflictId = await FieldTypeQuerier.FindIdAsync(fieldType.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(fieldType.Id))
      {
        throw new UniqueNameAlreadyUsedException(fieldType, conflictId.Value);
      }
    }

    await FieldTypeRepository.SaveAsync(fieldType, cancellationToken);
  }
}
