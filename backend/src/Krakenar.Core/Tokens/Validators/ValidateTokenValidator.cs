using FluentValidation;
using Krakenar.Contracts.Tokens;

namespace Krakenar.Core.Tokens.Validators;

public class ValidateTokenValidator : AbstractValidator<ValidateTokenPayload>
{
  public ValidateTokenValidator()
  {
    RuleFor(x => x.Token).NotEmpty();

    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).Secret());
  }
}
