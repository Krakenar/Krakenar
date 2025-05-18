using FluentValidation.Results;
using Krakenar.Contracts.Fields.Settings;

namespace Krakenar.Core.Fields.Validators;

public class DateTimeValueValidator : IFieldValueValidator
{
  protected virtual IDateTimeSettings Settings { get; }

  public DateTimeValueValidator(IDateTimeSettings settings)
  {
    Settings = settings;
  }

  public virtual Task<ValidationResult> ValidateAsync(FieldValue fieldValue, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 2);

    string value = fieldValue.Value;
    if (DateTime.TryParse(value, out DateTime dateTime))
    {
      if (dateTime < Settings.MinimumValue)
      {
        ValidationFailure failure = new(propertyName, $"The value must be greater than or equal to {Settings.MinimumValue}.", value)
        {
          CustomState = new { Settings.MinimumValue },
          ErrorCode = "GreaterThanOrEqualValidator"
        };
        failures.Add(failure);
      }
      if (dateTime > Settings.MaximumValue)
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
      ValidationFailure failure = new(propertyName, "The value is not a valid DateTime.", value)
      {
        ErrorCode = nameof(DateTimeValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
