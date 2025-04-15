using FluentValidation;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;

namespace Krakenar.Core.Validators;

public class CustomIdentifierValidator : AbstractValidator<CustomIdentifierDto>
{
  public CustomIdentifierValidator()
  {
    RuleFor(x => x.Key).Identifier();
    RuleFor(x => x.Value).CustomIdentifier();
  }
}
