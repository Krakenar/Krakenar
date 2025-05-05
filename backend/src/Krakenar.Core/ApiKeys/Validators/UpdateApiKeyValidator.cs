using FluentValidation;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Core.Roles.Validators;
using Krakenar.Core.Validators;

namespace Krakenar.Core.ApiKeys.Validators;

public class UpdateApiKeyValidator : AbstractValidator<UpdateApiKeyPayload>
{
  public UpdateApiKeyValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).Future());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
    RuleForEach(x => x.Roles).SetValidator(new RoleChangeValidator());
  }
}
