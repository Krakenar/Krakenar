using FluentValidation.Results;
using Krakenar.Core.Fields.Settings;

namespace Krakenar.Core.Fields.Validators;

public class SelectValueValidator : IFieldValueValidator
{
  protected virtual SelectSettings Settings { get; }

  public SelectValueValidator(SelectSettings settings)
  {
    Settings = settings;
  }

  public virtual Task<ValidationResult> ValidateAsync(FieldValue field, string propertyName, CancellationToken cancellationToken)
  {
    string fieldValue = field.Value;
    IReadOnlyCollection<string>? values = null;
    try
    {
      values = JsonSerializer.Deserialize<IReadOnlyCollection<string>>(fieldValue);
    }
    catch (Exception)
    {
    }
    if (values is null || values.Count < 1)
    {
      ValidationFailure failure = new(propertyName, "The value must be a JSON-serialized non-empty string array.", fieldValue)
      {
        ErrorCode = nameof(SelectValueValidator)
      };
      return Task.FromResult(new ValidationResult([failure]));
    }
    else if (values.Count > 1 && !Settings.IsMultiple)
    {
      ValidationFailure failure = new(propertyName, "Only one option may be selected.", fieldValue)
      {
        ErrorCode = "MultipleValidator"
      };
      return Task.FromResult(new ValidationResult([failure]));
    }

    List<ValidationFailure> failures = new(capacity: values.Count);

    HashSet<string> allowedValues = [.. Settings.Options.Select(option => option.Value ?? option.Text)];
    foreach (string value in values)
    {
      if (!allowedValues.Contains(value))
      {
        ValidationFailure failure = new(propertyName, $"The value should be one of the following: {string.Join(", ", allowedValues)}.", value)
        {
          CustomState = new { AllowedValues = allowedValues },
          ErrorCode = "OptionValidator"
        };
        failures.Add(failure);
      }
    }

    return Task.FromResult(new ValidationResult(failures));
  }
}
