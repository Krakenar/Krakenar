using FluentValidation;
using Krakenar.Core.Fields.Events;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;

namespace Krakenar.Core.Fields;

public interface IFieldManager
{
  Task<FieldType> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken = default);
  Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken = default);
}

public class FieldManager : IFieldManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IFieldTypeQuerier FieldTypeQuerier { get; }
  protected virtual IFieldTypeRepository FieldTypeRepository { get; }

  public FieldManager(IApplicationContext applicationContext, IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository)
  {
    ApplicationContext = applicationContext;
    FieldTypeQuerier = fieldTypeQuerier;
    FieldTypeRepository = fieldTypeRepository;
  }

  public virtual async Task<FieldType> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken)
  {
    RealmId? realmId = ApplicationContext.RealmId;
    FieldType? fieldType = null;

    if (Guid.TryParse(idOrUniqueName, out Guid entityId))
    {
      FieldTypeId fieldTypeId = new(entityId, realmId);
      fieldType = await FieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    }

    if (fieldType is null)
    {
      try
      {
        UniqueNameSettings settings = new(allowedCharacters: null);
        UniqueName uniqueName = new(settings, idOrUniqueName);
        FieldTypeId? fieldTypeId = await FieldTypeQuerier.FindIdAsync(uniqueName, cancellationToken);
        if (fieldTypeId.HasValue)
        {
          fieldType = await FieldTypeRepository.LoadAsync(fieldTypeId.Value, cancellationToken);
        }
      }
      catch (ValidationException)
      {
      }
    }

    return fieldType ?? throw new FieldTypeNotFoundException(realmId, idOrUniqueName, propertyName);
  }

  public virtual async Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = fieldType.Changes.Any(change => change is FieldTypeCreated || change is FieldTypeUniqueNameChanged);
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
