using FluentValidation;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Validators;

namespace Krakenar.Core.Sessions.Validators;

public class SignInSessionValidator : AbstractValidator<SignInSessionPayload>
{
  public SignInSessionValidator()
  {
    RuleFor(x => x.UniqueName).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
