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
        break;
      case DataType.DateTime:
        When(x => x.DateTime is not null, () => RuleFor(x => x.DateTime!).SetValidator(new DateTimeSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.DateTime).NotNull());
        break;
      case DataType.Number:
        When(x => x.Number is not null, () => RuleFor(x => x.Number!).SetValidator(new NumberSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.Number).NotNull());
        break;
      case DataType.RelatedContent:
        When(x => x.RelatedContent is not null, () => RuleFor(x => x.RelatedContent!).SetValidator(new RelatedContentSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.RelatedContent).NotNull());
        break;
      case DataType.RichText:
        When(x => x.RichText is not null, () => RuleFor(x => x.RichText!).SetValidator(new RichTextSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.RichText).NotNull());
        break;
      case DataType.Select:
        When(x => x.Select is not null, () => RuleFor(x => x.Select!).SetValidator(new SelectSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.Select).NotNull());
        break;
      case DataType.String:
        When(x => x.String is not null, () => RuleFor(x => x.String!).SetValidator(new StringSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.String).NotNull());
        break;
      case DataType.Tags:
        When(x => x.Tags is not null, () => RuleFor(x => x.Tags!).SetValidator(new TagsSettingsValidator()))
          .Otherwise(() => RuleFor(x => x.Tags).NotNull());
        break;
      default:
        throw new DataTypeNotSupportedException(type);
    }

    if (type != DataType.Boolean)
    {
      RuleFor(x => x.Boolean).Null();
    }
    if (type != DataType.DateTime)
    {
      RuleFor(x => x.DateTime).Null();
    }
    if (type != DataType.Number)
    {
      RuleFor(x => x.Number).Null();
    }
    if (type != DataType.RelatedContent)
    {
      RuleFor(x => x.RelatedContent).Null();
    }
    if (type != DataType.RichText)
    {
      RuleFor(x => x.RichText).Null();
    }
    if (type != DataType.Select)
    {
      RuleFor(x => x.Select).Null();
    }
    if (type != DataType.String)
    {
      RuleFor(x => x.String).Null();
    }
    if (type != DataType.Tags)
    {
      RuleFor(x => x.Tags).Null();
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
