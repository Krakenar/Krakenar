using FluentValidation;
using Krakenar.Contracts.Realms;
using Krakenar.Core.Settings.Validators;
using Krakenar.Core.Validators;

namespace Krakenar.Core.Realms.Validators;

public class CreateOrReplaceRealmValidator : AbstractValidator<CreateOrReplaceRealmPayload>
{
  public CreateOrReplaceRealmValidator()
  {
    RuleFor(x => x.UniqueSlug).Slug();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());

    RuleFor(x => x.UniqueNameSettings).SetValidator(new UniqueNameSettingsValidator());
    RuleFor(x => x.PasswordSettings).SetValidator(new PasswordSettingsValidator());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
