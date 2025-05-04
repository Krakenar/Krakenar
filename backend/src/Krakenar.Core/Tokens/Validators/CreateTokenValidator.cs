using FluentValidation;
using Krakenar.Contracts.Tokens;
using Krakenar.Core.Users.Validators;

namespace Krakenar.Core.Tokens.Validators;

public class CreateTokenValidator : AbstractValidator<CreateTokenPayload>
{
  public CreateTokenValidator()
  {
    When(x => x.LifetimeSeconds.HasValue, () => RuleFor(x => x.LifetimeSeconds!.Value).GreaterThan(0));
    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).Secret());

    When(x => x.Email is not null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    RuleForEach(x => x.Claims).SetValidator(new ClaimValidator());
  }
}
