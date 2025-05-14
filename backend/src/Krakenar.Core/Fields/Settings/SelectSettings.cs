using FluentValidation;
using Krakenar.Contracts.Fields;

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

  private class Validator : AbstractValidator<SelectSettings>;
}
