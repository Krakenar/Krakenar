using FluentValidation.Results;
using Krakenar.Contracts.Fields.Settings;

namespace Krakenar.Core.Fields.Validators;

public class StringValueValidator : IFieldValueValidator
{
  protected virtual IStringSettings Settings { get; }

  public StringValueValidator(IStringSettings settings)
  {
    Settings = settings;
  }

  public virtual Task<ValidationResult> ValidateAsync(FieldValue fieldValue, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 3);

    string value = fieldValue.Value;
    if (value.Length < Settings.MinimumLength)
    {
      ValidationFailure failure = new(propertyName, $"The length of the value must be at least {Settings.MinimumLength} characters.", value)
      {
        CustomState = new { Settings.MinimumLength },
        ErrorCode = "MinimumLengthValidator"
      };
      failures.Add(failure);
    }
    if (value.Length > Settings.MaximumLength)
    {
      ValidationFailure failure = new(propertyName, $"The length of the value may not exceed {Settings.MaximumLength} characters.", value)
      {
        CustomState = new { Settings.MaximumLength },
        ErrorCode = "MaximumLengthValidator"
      };
      failures.Add(failure);
    }
    if (Settings.Pattern is not null && !Regex.IsMatch(value, Settings.Pattern))
    {
      ValidationFailure failure = new(propertyName, $"The value must match the pattern '{Settings.Pattern}'.", value)
      {
        CustomState = new { Settings.Pattern },
        ErrorCode = "RegularExpressionValidator"
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
