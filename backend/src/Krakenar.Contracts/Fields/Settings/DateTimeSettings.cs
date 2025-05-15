
namespace Krakenar.Contracts.Fields.Settings;

public record DateTimeSettings : IDateTimeSettings
{
  public DateTime? MinimumValue { get; set; }
  public DateTime? MaximumValue { get; set; }

  public DateTimeSettings()
  {
  }

  [JsonConstructor]
  public DateTimeSettings(DateTime? minimumValue, DateTime? maximumValue)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
  }

  public DateTimeSettings(IDateTimeSettings dateTime) : this(dateTime.MinimumValue, dateTime.MaximumValue)
  {
  }
}
