using FluentValidation;
using Krakenar.Contracts.Fields;
using SelectSettingsDto = Krakenar.Contracts.Fields.Settings.SelectSettings;

namespace Krakenar.Core.Fields.Settings;

public record SelectSettings : FieldTypeSettings
{
  public override DataType DataType => DataType.Select;

  public IReadOnlyCollection<SelectOption> Options { get; }
  public bool IsMultiple { get; }

  public SelectSettings() : this(options: [])
  {
  }

  [JsonConstructor]
  public SelectSettings(IReadOnlyCollection<SelectOption> options, bool isMultiple = false)
  {
    Options = options.ToList().AsReadOnly();
    IsMultiple = isMultiple;
    new Validator().ValidateAndThrow(this);
  }

  public SelectSettings(SelectSettingsDto select)
    : this([.. select.Options.Select(option => new SelectOption(option))], select.IsMultiple)
  {
  }

  private class Validator : AbstractValidator<SelectSettings>;
}
