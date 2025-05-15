namespace Krakenar.Contracts.Fields.Settings;

public interface ISelectOption
{
  string Text { get; }
  string? Value { get; }
  string? Label { get; }
  bool IsDisabled { get; }
}
