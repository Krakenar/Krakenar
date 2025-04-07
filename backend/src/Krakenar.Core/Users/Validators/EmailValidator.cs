using FluentValidation;
using Krakenar.Contracts.Users;

namespace Krakenar.Core.Users.Validators;

public class EmailValidator : AbstractValidator<IEmail>
{
  public EmailValidator()
  {
    RuleFor(x => x.Address).NotEmpty().MaximumLength(Email.MaximumLength).EmailAddress();
  }
}
