using FluentValidation;
using Krakenar.Contracts.Users;

namespace Krakenar.Core.Users.Validators;

public class AuthenticateUserValidator : AbstractValidator<AuthenticateUserPayload>
{
  public AuthenticateUserValidator()
  {
    RuleFor(x => x.User).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
  }
}
