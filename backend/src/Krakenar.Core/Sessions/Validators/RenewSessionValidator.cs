using FluentValidation;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Validators;

namespace Krakenar.Core.Sessions.Validators;

public class RenewSessionValidator : AbstractValidator<RenewSessionPayload>
{
  public RenewSessionValidator()
  {
    RuleFor(x => x.RefreshToken).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
