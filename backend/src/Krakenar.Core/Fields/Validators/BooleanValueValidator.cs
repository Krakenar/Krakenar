using FluentValidation.Results;
using Krakenar.Contracts.Fields.Settings;

namespace Krakenar.Core.Fields.Validators;

public class BooleanValueValidator : IFieldValueValidator
{
  protected virtual IBooleanSettings Settings { get; }

  public BooleanValueValidator(IBooleanSettings settings)
  {
    Settings = settings;
  }

  public virtual Task<ValidationResult> ValidateAsync(FieldValue fieldValue, string propertyName, CancellationToken cancellationToken)
  {
    List<ValidationFailure> failures = new(capacity: 1);

    string value = fieldValue.Value;
    if (!bool.TryParse(value, out _))
    {
      ValidationFailure failure = new(propertyName, "The value is not a valid boolean.", value)
      {
        ErrorCode = nameof(BooleanValueValidator)
      };
      failures.Add(failure);
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
