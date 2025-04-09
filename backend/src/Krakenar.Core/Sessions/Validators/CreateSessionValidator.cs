using FluentValidation;
using Krakenar.Contracts.Sessions;
using Krakenar.Core.Validators;

namespace Krakenar.Core.Sessions.Validators;

public class CreateSessionValidator : AbstractValidator<CreateSessionPayload>
{
  public CreateSessionValidator()
  {
    RuleFor(x => x.User).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
