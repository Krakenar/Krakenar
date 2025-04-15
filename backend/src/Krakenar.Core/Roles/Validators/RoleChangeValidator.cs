using FluentValidation;
using Krakenar.Contracts.Roles;

namespace Krakenar.Core.Roles.Validators;

public class RoleChangeValidator : AbstractValidator<RoleChange>
{
  public RoleChangeValidator()
  {
    RuleFor(x => x.Action).IsInEnum();
  }
}
