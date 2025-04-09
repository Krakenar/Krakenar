using FluentValidation;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Validators;

namespace Krakenar.Core.Roles.Validators;

public class UpdateRoleValidator : AbstractValidator<UpdateRolePayload>
{
  public UpdateRoleValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
