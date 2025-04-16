using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Users;

namespace Krakenar.Core.Users.Validators;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordPayload>
{
  public ChangePasswordValidator(IPasswordSettings passwordSettings)
  {
    When(x => x.Current is not null, () => RuleFor(x => x.Current).NotEmpty());
    RuleFor(x => x.New).Password(passwordSettings);
  }
}
