
namespace Krakenar.Contracts.Fields.Settings;

public record NumberSettings : INumberSettings
{
  public double? MinimumValue { get; set; }
  public double? MaximumValue { get; set; }
  public double? Step { get; set; }

  public NumberSettings()
  {
  }

  [JsonConstructor]
  public NumberSettings(double? minimumValue, double? maximumValue, double? step)
  {
    MinimumValue = minimumValue;
    MaximumValue = maximumValue;
    Step = step;
  }

  public NumberSettings(INumberSettings number) : this(number.MinimumValue, number.MaximumValue, number.Step)
  {
  }
}
