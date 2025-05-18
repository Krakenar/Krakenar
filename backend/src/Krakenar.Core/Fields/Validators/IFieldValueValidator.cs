using FluentValidation.Results;

namespace Krakenar.Core.Fields.Validators;

public interface IFieldValueValidator
{
  Task<ValidationResult> ValidateAsync(FieldValue fieldValue, string propertyName, CancellationToken cancellationToken = default);
}
