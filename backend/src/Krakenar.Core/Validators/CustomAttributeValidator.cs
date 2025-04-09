using FluentValidation;
using Krakenar.Contracts;

namespace Krakenar.Core.Validators;

public class CustomAttributeValidator : AbstractValidator<CustomAttribute>
{
  public CustomAttributeValidator()
  {
    RuleFor(x => x.Key).Identifier();
  }
}
