using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Fields.Validators;

namespace Krakenar.Core.Fields.Settings;

public record StringSettings : FieldTypeSettings, IStringSettings
{
  public override DataType DataType => DataType.String;

  public int? MinimumLength { get; }
  public int? MaximumLength { get; }
  public string? Pattern { get; }

  public StringSettings()
  {
  }

  [JsonConstructor]
  public StringSettings(int? minimumLength, int? maximumLength, string? pattern)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    Pattern = pattern;
    new StringSettingsValidator().ValidateAndThrow(this);
  }

  public StringSettings(IStringSettings @string) : this(@string.MinimumLength, @string.MaximumLength, @string.Pattern)
  {
  }
}
