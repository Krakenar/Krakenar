using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Fields.Settings;
using Krakenar.Core.Fields.Validators;
using Logitar.EventSourcing;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.Core.Fields.Commands;

public record UpdateFieldType(Guid Id, UpdateFieldTypePayload Payload) : ICommand<FieldTypeDto?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateFieldTypeHandler : ICommandHandler<UpdateFieldType, FieldTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IFieldManager FieldManager { get; }
  protected virtual IFieldTypeQuerier FieldTypeQuerier { get; }
  protected virtual IFieldTypeRepository FieldTypeRepository { get; }

  public UpdateFieldTypeHandler(
    IApplicationContext applicationContext,
    IFieldManager fieldManager,
    IFieldTypeQuerier fieldTypeQuerier,
    IFieldTypeRepository fieldTypeRepository)
  {
    ApplicationContext = applicationContext;
    FieldManager = fieldManager;
    FieldTypeQuerier = fieldTypeQuerier;
    FieldTypeRepository = fieldTypeRepository;
  }

  public virtual async Task<FieldTypeDto?> HandleAsync(UpdateFieldType command, CancellationToken cancellationToken)
  {
    FieldTypeId fieldTypeId = new(command.Id, ApplicationContext.RealmId);
    FieldType? fieldType = await FieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    if (fieldType is null)
    {
      return null;
    }

    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    UpdateFieldTypePayload payload = command.Payload;
    new UpdateFieldTypeValidator(uniqueNameSettings, fieldType.DataType).ValidateAndThrow(payload);

    ActorId? actorId = ApplicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      fieldType.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      fieldType.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      fieldType.Description = Description.TryCreate(payload.Description.Value);
    }

    if (payload.Boolean is not null)
    {
      BooleanSettings settings = new(payload.Boolean);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.DateTime is not null)
    {
      DateTimeSettings settings = new(payload.DateTime);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.Number is not null)
    {
      NumberSettings settings = new(payload.Number);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.RelatedContent is not null)
    {
      RelatedContentSettings settings = new(payload.RelatedContent);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.RichText is not null)
    {
      RichTextSettings settings = new(payload.RichText);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.Select is not null)
    {
      SelectSettings settings = new(payload.Select);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.String is not null)
    {
      StringSettings settings = new(payload.String);
      fieldType.SetSettings(settings, actorId);
    }
    if (payload.Tags is not null)
    {
      TagsSettings settings = new(payload.Tags);
      fieldType.SetSettings(settings, actorId);
    }

    fieldType.Update(actorId);
    await FieldManager.SaveAsync(fieldType, cancellationToken);

    return await FieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
  }
}
