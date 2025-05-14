using FluentValidation;
using Logitar;

namespace Krakenar.Core.Fields.Settings;

public record SelectOption
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
    new Validator().ValidateAndThrow(this);
  }

  private class Validator : AbstractValidator<SelectOption>
  {
    public Validator()
    {
      RuleFor(x => x.Text).NotEmpty();
    }
  }
}
