using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Fields.Validators;

namespace Krakenar.Core.Fields.Settings;

public record NumberSettings : FieldTypeSettings, INumberSettings
{
  public override DataType DataType => DataType.Number;

  public double? MinimumValue { get; }
  public double? MaximumValue { get; }
  public double? Step { get; }

  public NumberSettings()
  {
  }

  [JsonConstructor]
  public NumberSettings(double? minimumValue, double? maximumValue, double? step)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    Step = step;
    new NumberSettingsValidator().ValidateAndThrow(this);
  }

  public NumberSettings(INumberSettings number) : this(number.MinimumValue, number.MaximumValue, number.Step)
  {
  }
}
