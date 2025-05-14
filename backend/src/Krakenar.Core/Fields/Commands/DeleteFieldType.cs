using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.Core.Fields.Commands;

public record DeleteFieldType(Guid Id) : ICommand<FieldTypeDto?>;

public class DeleteFieldTypeHandler : ICommandHandler<DeleteFieldType, FieldTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IFieldTypeQuerier FieldTypeQuerier { get; }
  protected virtual IFieldTypeRepository FieldTypeRepository { get; }

  public DeleteFieldTypeHandler(IApplicationContext applicationContext, IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository)
  {
    ApplicationContext = applicationContext;
    FieldTypeQuerier = fieldTypeQuerier;
    FieldTypeRepository = fieldTypeRepository;
  }

  public virtual async Task<FieldTypeDto?> HandleAsync(DeleteFieldType command, CancellationToken cancellationToken)
  {
    FieldTypeId fieldTypeId = new(command.Id, ApplicationContext.RealmId);
    FieldType? fieldType = await FieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    if (fieldType is null)
    {
      return null;
    }
    FieldTypeDto dto = await FieldTypeQuerier.ReadAsync(fieldType, cancellationToken);

    fieldType.Delete(ApplicationContext.ActorId);
    await FieldTypeRepository.SaveAsync(fieldType, cancellationToken);

    return dto;
  }
}
