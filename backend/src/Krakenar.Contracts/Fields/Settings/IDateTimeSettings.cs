namespace Krakenar.Contracts.Fields.Settings;

public interface IDateTimeSettings
{
  DateTime? MinimumValue { get; }
  DateTime? MaximumValue { get; }
}
