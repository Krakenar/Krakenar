using FluentValidation.Results;
using Krakenar.Contracts.Fields.Settings;

namespace Krakenar.Core.Fields.Validators;

public class NumberValueValidator : IFieldValueValidator
{
  protected virtual INumberSettings Settings { get; }

  public NumberValueValidator(INumberSettings settings)
  {
    Settings = settings;
  }

  public virtual Task<ValidationResult> ValidateAsync(FieldValue fieldValue, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 2);

    string value = fieldValue.Value;
    if (double.TryParse(value, out double number))
    {
      if (number < Settings.MinimumValue)
      {
        ValidationFailure failure = new(propertyName, $"The value must be greater than or equal to {Settings.MinimumValue}.", value)
        {
          CustomState = new { Settings.MinimumValue },
          ErrorCode = "GreaterThanOrEqualValidator"
        };
        failures.Add(failure);
      }
      if (number > Settings.MaximumValue)
      {
        ValidationFailure failure = new(propertyName, $"The value must be less than or equal to {Settings.MaximumValue}.", value)
        {
          CustomState = new { Settings.MaximumValue },
          ErrorCode = "LessThanOrEqualValidator"
        };
        failures.Add(failure);
      }
    }
    else
    {
      ValidationFailure failure = new(propertyName, "The value is not a valid number.", value)
      {
        ErrorCode = nameof(NumberValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
