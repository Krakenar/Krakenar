using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Fields.Validators;

namespace Krakenar.Core.Fields.Settings;

public record TagsSettings : FieldTypeSettings, ITagsSettings
{
  public override DataType DataType => DataType.Tags;

  [JsonConstructor]
  public TagsSettings()
  {
    new TagsSettingsValidator().ValidateAndThrow(this);
  }

  public TagsSettings(ITagsSettings _) : this()
  {
  }
}
