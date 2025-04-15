using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Users;

namespace Krakenar.Core.Users.Validators;

public class ResetUserPasswordValidator : AbstractValidator<ResetUserPasswordPayload>
{
  public ResetUserPasswordValidator(IPasswordSettings passwordSettings)
  {
    RuleFor(x => x.Password).Password(passwordSettings);
  }
}
