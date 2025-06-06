﻿using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Fields.Validators;

public class UpdateFieldTypeValidator : AbstractValidator<UpdateFieldTypePayload>
{
  public UpdateFieldTypeValidator(IUniqueNameSettings uniqueNameSettings, DataType dataType)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    switch (dataType)
    {
      case DataType.Boolean:
        When(x => x.Boolean is not null, () => RuleFor(x => x.Boolean!).SetValidator(new BooleanSettingsValidator()));
        break;
      case DataType.DateTime:
        When(x => x.DateTime is not null, () => RuleFor(x => x.DateTime!).SetValidator(new DateTimeSettingsValidator()));
        break;
      case DataType.Number:
        When(x => x.Number is not null, () => RuleFor(x => x.Number!).SetValidator(new NumberSettingsValidator()));
        break;
      case DataType.RelatedContent:
        When(x => x.RelatedContent is not null, () => RuleFor(x => x.RelatedContent!).SetValidator(new RelatedContentSettingsValidator()));
        break;
      case DataType.RichText:
        When(x => x.RichText is not null, () => RuleFor(x => x.RichText!).SetValidator(new RichTextSettingsValidator()));
        break;
      case DataType.Select:
        When(x => x.Select is not null, () => RuleFor(x => x.Select!).SetValidator(new SelectSettingsValidator()));
        break;
      case DataType.String:
        When(x => x.String is not null, () => RuleFor(x => x.String!).SetValidator(new StringSettingsValidator()));
        break;
      case DataType.Tags:
        When(x => x.Tags is not null, () => RuleFor(x => x.Tags!).SetValidator(new TagsSettingsValidator()));
        break;
      default:
        throw new DataTypeNotSupportedException(dataType);
    }

    if (dataType != DataType.Boolean)
    {
      RuleFor(x => x.Boolean).Null();
    }
    if (dataType != DataType.DateTime)
    {
      RuleFor(x => x.DateTime).Null();
    }
    if (dataType != DataType.Number)
    {
      RuleFor(x => x.Number).Null();
    }
    if (dataType != DataType.RelatedContent)
    {
      RuleFor(x => x.RelatedContent).Null();
    }
    if (dataType != DataType.RichText)
    {
      RuleFor(x => x.RichText).Null();
    }
    if (dataType != DataType.Select)
    {
      RuleFor(x => x.Select).Null();
    }
    if (dataType != DataType.String)
    {
      RuleFor(x => x.String).Null();
    }
    if (dataType != DataType.Tags)
    {
      RuleFor(x => x.Tags).Null();
    }
  }
}
