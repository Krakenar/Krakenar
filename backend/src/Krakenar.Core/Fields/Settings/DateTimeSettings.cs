using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Fields.Validators;

namespace Krakenar.Core.Fields.Settings;

public record DateTimeSettings : FieldTypeSettings, IDateTimeSettings
{
  public override DataType DataType => DataType.DateTime;

  public DateTime? MinimumValue { get; }
  public DateTime? MaximumValue { get; }

  public DateTimeSettings()
  {
  }

  [JsonConstructor]
  public DateTimeSettings(DateTime? minimumValue, DateTime? maximumValue)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    new DateTimeSettingsValidator().ValidateAndThrow(this);
  }

  public DateTimeSettings(IDateTimeSettings dateTime) : this(dateTime.MinimumValue, dateTime.MaximumValue)
  {
  }
}
