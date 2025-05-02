using FluentValidation;
using Claim = Krakenar.Contracts.Tokens.Claim;

namespace Krakenar.Core.Tokens.Validators;

public class ClaimValidator : AbstractValidator<Claim>
{
  public ClaimValidator()
  {
    RuleFor(x => x.Name).Identifier();
    RuleFor(x => x.Value).NotEmpty();
  }
}
