using FluentValidation;
using Krakenar.Contracts.ApiKeys;
using Krakenar.Core.Validators;

namespace Krakenar.Core.ApiKeys.Validators;

public class CreateOrReplaceApiKeyValidator : AbstractValidator<CreateOrReplaceApiKeyPayload>
{
  public CreateOrReplaceApiKeyValidator()
  {
    RuleFor(x => x.Name).DisplayName();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).Future());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
    RuleForEach(x => x.Roles).NotEmpty();
  }
}
