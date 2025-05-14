using FluentValidation;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Fields.Validators;
using Logitar;

namespace Krakenar.Core.Fields.Settings;

public record SelectOption : ISelectOption
{
  public string Text { get; }
  public string? Value { get; }
  public string? Label { get; }
  public bool IsDisabled { get; }

  [JsonConstructor]
  public SelectOption(string text, string? value = null, string? label = null, bool isDisabled = false)
  {
    Text = text.Trim();
    Value = value?.CleanTrim();
    Label = label?.CleanTrim();
    IsDisabled = isDisabled;
    new SelectOptionValidator().ValidateAndThrow(this);
  }

  public SelectOption(ISelectOption option) : this(option.Text, option.Value, option.Label, option.IsDisabled)
  {
  }
}
