using FluentValidation;
using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Settings;
using Krakenar.Core.Validators;

namespace Krakenar.Core.Roles.Validators;

public class CreateOrReplaceRoleValidator : AbstractValidator<CreateOrReplaceRolePayload>
{
  public CreateOrReplaceRoleValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
