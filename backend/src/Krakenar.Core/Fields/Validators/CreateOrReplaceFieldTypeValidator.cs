using FluentValidation;
using FluentValidation.Results;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Fields.Validators;

public class CreateOrReplaceFieldTypeValidator : AbstractValidator<CreateOrReplaceFieldTypePayload>
{
  protected virtual bool IsCreate { get; }

  public CreateOrReplaceFieldTypeValidator(IUniqueNameSettings uniqueNameSettings, DataType? dataType)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    if (dataType.HasValue)
    {
      ConfigureReplace(dataType.Value);
    }
    else
    {
      IsCreate = true;
      ConfigureCreate();
    }
  }

  public override ValidationResult Validate(ValidationContext<CreateOrReplaceFieldTypePayload> context)
  {
    List<ValidationFailure> failures = new(capacity: 1);
    if (IsCreate)
    {
      CreateOrReplaceFieldTypePayload payload = context.InstanceToValidate;
      DataType? type = GetDataType(payload);
      if (type is null)
      {
        string properties = string.Join(", ", Enum.GetNames<DataType>());
        ValidationFailure failure = new()
        {
          ErrorCode = nameof(CreateOrReplaceFieldTypeValidator),
          ErrorMessage = $"Exactly one of the following properties must be provided: {properties}."
        };
        failures.Add(failure);
      }
    }

    ValidationResult result = base.Validate(context);
    return new ValidationResult(failures.Concat(result.Errors));
  }

  protected virtual void ConfigureCreate()
  {
    When(x => x.Boolean is not null, () => RuleFor(x => x.Boolean!).SetValidator(new BooleanSettingsValidator()));
    When(x => x.DateTime is not null, () => RuleFor(x => x.DateTime!).SetValidator(new DateTimeSettingsValidator()));
    When(x => x.Number is not null, () => RuleFor(x => x.Number!).SetValidator(new NumberSettingsValidator()));
    When(x => x.RelatedContent is not null, () => RuleFor(x => x.RelatedContent!).SetValidator(new RelatedContentSettingsValidator()));
    When(x => x.RichText is not null, () => RuleFor(x => x.RichText!).SetValidator(new RichTextSettingsValidator()));
    When(x => x.Select is not null, () => RuleFor(x => x.Select!).SetValidator(new SelectSettingsValidator()));
    When(x => x.String is not null, () => RuleFor(x => x.String!).SetValidator(new StringSettingsValidator()));
    When(x => x.Tags is not null, () => RuleFor(x => x.Tags!).SetValidator(new TagsSettingsValidator()));
  }

  protected virtual void ConfigureReplace(DataType type)
  {
    switch (type)
    {
      case DataType.Boolean:
        When(x => x.Boolean is not null, () => RuleFor(x => x.Boolean!).SetValidator(new BooleanSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.Boolean).NotNull());
        RuleFor(x => x.DateTime).Null();
        RuleFor(x => x.Number).Null();
        RuleFor(x => x.RelatedContent).Null();
        RuleFor(x => x.RichText).Null();
        RuleFor(x => x.Select).Null();
        RuleFor(x => x.String).Null();
        RuleFor(x => x.Tags).Null();
        break;
      // TODO(fpion): complete
      default:
        throw new DataTypeNotSupportedException(type);
    }
  }

  protected virtual DataType? GetDataType(CreateOrReplaceFieldTypePayload payload)
  {
    List<DataType> types = new(capacity: 8);
    if (payload.Boolean is not null)
    {
      types.Add(DataType.Boolean);
    }
    if (payload.DateTime is not null)
    {
      types.Add(DataType.DateTime);
    }
    if (payload.Number is not null)
    {
      types.Add(DataType.Number);
    }
    if (payload.RelatedContent is not null)
    {
      types.Add(DataType.RelatedContent);
    }
    if (payload.RichText is not null)
    {
      types.Add(DataType.RichText);
    }
    if (payload.Select is not null)
    {
      types.Add(DataType.Select);
    }
    if (payload.String is not null)
    {
      types.Add(DataType.String);
    }
    if (payload.Tags is not null)
    {
      types.Add(DataType.Tags);
    }
    return types.Count == 1 ? types.Single() : null;
  }
}
