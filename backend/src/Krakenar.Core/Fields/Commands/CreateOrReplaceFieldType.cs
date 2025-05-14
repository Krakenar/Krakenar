using FluentValidation;
using FluentValidation.Results;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Fields.Settings;
using Krakenar.Core.Fields.Validators;
using Logitar.EventSourcing;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.Core.Fields.Commands;

public record CreateOrReplaceFieldType(Guid? Id, CreateOrReplaceFieldTypePayload Payload, long? Version) : ICommand<CreateOrReplaceFieldTypeResult>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceFieldTypeHandler : ICommandHandler<CreateOrReplaceFieldType, CreateOrReplaceFieldTypeResult>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IFieldManager FieldManager { get; }
  protected virtual IFieldTypeQuerier FieldTypeQuerier { get; }
  protected virtual IFieldTypeRepository FieldTypeRepository { get; }

  public CreateOrReplaceFieldTypeHandler(
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

  public virtual async Task<CreateOrReplaceFieldTypeResult> HandleAsync(CreateOrReplaceFieldType command, CancellationToken cancellationToken)
  {
    FieldTypeId fieldTypeId = FieldTypeId.NewId(ApplicationContext.RealmId);
    FieldType? fieldType = null;
    if (command.Id.HasValue)
    {
      fieldTypeId = new(command.Id.Value, fieldTypeId.RealmId);
      fieldType = await FieldTypeRepository.LoadAsync(fieldTypeId, cancellationToken);
    }

    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    CreateOrReplaceFieldTypePayload payload = command.Payload;
    ValidationResult validation = new CreateOrReplaceFieldTypeValidator(uniqueNameSettings, fieldType?.DataType).Validate(payload);
    if (!validation.IsValid)
    {
      throw new ValidationException(validation.Errors);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    FieldTypeSettings settings = GetSettings(payload);
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    if (fieldType is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceFieldTypeResult();
      }

      fieldType = new(uniqueName, settings, actorId, fieldTypeId);
      created = true;
    }

    FieldType reference = (command.Version.HasValue
      ? await FieldTypeRepository.LoadAsync(fieldTypeId, command.Version, cancellationToken)
      : null) ?? fieldType;

    if (reference.UniqueName != uniqueName)
    {
      fieldType.SetUniqueName(uniqueName, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      fieldType.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      fieldType.Description = description;
    }

    if (reference.Settings != settings)
    {
      switch (settings.DataType)
      {
        case DataType.Boolean:
          fieldType.SetSettings((BooleanSettings)settings);
          break;
        case DataType.DateTime:
          fieldType.SetSettings((DateTimeSettings)settings);
          break;
        case DataType.Number:
          fieldType.SetSettings((NumberSettings)settings);
          break;
        case DataType.RelatedContent:
          fieldType.SetSettings((RelatedContentSettings)settings);
          break;
        case DataType.RichText:
          fieldType.SetSettings((RichTextSettings)settings);
          break;
        case DataType.Select:
          fieldType.SetSettings((SelectSettings)settings);
          break;
        case DataType.String:
          fieldType.SetSettings((StringSettings)settings);
          break;
        case DataType.Tags:
          fieldType.SetSettings((TagsSettings)settings);
          break;
        default:
          throw new DataTypeNotSupportedException(settings.DataType);
      }
    }

    fieldType.Update(actorId);
    await FieldManager.SaveAsync(fieldType, cancellationToken);

    FieldTypeDto dto = await FieldTypeQuerier.ReadAsync(fieldType, cancellationToken);
    return new CreateOrReplaceFieldTypeResult(dto, created);
  }

  protected virtual FieldTypeSettings GetSettings(CreateOrReplaceFieldTypePayload payload)
  {
    List<FieldTypeSettings> settings = new(capacity: 8);

    if (payload.Boolean is not null)
    {
      settings.Add(new BooleanSettings(payload.Boolean));
    }
    if (payload.DateTime is not null)
    {
      settings.Add(new DateTimeSettings(payload.DateTime));
    }
    if (payload.Number is not null)
    {
      settings.Add(new NumberSettings(payload.Number));
    }
    if (payload.RelatedContent is not null)
    {
      settings.Add(new RelatedContentSettings(payload.RelatedContent));
    }
    if (payload.RichText is not null)
    {
      settings.Add(new RichTextSettings(payload.RichText));
    }
    if (payload.Select is not null)
    {
      settings.Add(new SelectSettings(payload.Select));
    }
    if (payload.String is not null)
    {
      settings.Add(new StringSettings(payload.String));
    }
    if (payload.Tags is not null)
    {
      settings.Add(new TagsSettings(payload.Tags));
    }

    if (settings.Count > 1)
    {
      throw new ArgumentException($"Exactly one setting property must be set; {settings.Count} were set.", nameof(payload));
    }
    else if (settings.Count < 1)
    {
      throw new ArgumentException("Exactly one setting property must be set; none were set.", nameof(payload));
    }

    return settings.Single();
  }
}
