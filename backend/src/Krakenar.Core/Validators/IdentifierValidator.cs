using FluentValidation;
using FluentValidation.Validators;

namespace Krakenar.Core.Validators;

public class IdentifierValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "IdentifierValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' may only contain letters, digits and underscores (_), and must not start with a digit.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    return string.IsNullOrEmpty(value) || (!char.IsDigit(value.First()) && value.All(c => char.IsLetterOrDigit(c) || c == '_'));
  }
}
