using FluentValidation;
using Krakenar.Contracts.Passwords;
using Krakenar.Core.Validators;

namespace Krakenar.Core.Passwords.Validators;

public class ValidateOneTimePasswordValidator : AbstractValidator<ValidateOneTimePasswordPayload>
{
  public ValidateOneTimePasswordValidator()
  {
    RuleFor(x => x.Password).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
