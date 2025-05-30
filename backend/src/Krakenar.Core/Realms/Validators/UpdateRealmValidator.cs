using FluentValidation;
using Krakenar.Contracts.Realms;
using Krakenar.Core.Settings.Validators;
using Krakenar.Core.Validators;

namespace Krakenar.Core.Realms.Validators;

public class UpdateRealmValidator : AbstractValidator<UpdateRealmPayload>
{
  public UpdateRealmValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueSlug), () => RuleFor(x => x.UniqueSlug!).Slug());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.Secret?.Value), () => RuleFor(x => x.Secret!.Value!).Secret());

    When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());

    When(x => x.UniqueNameSettings is not null, () => RuleFor(x => x.UniqueNameSettings!).SetValidator(new UniqueNameSettingsValidator()));
    When(x => x.PasswordSettings is not null, () => RuleFor(x => x.PasswordSettings!).SetValidator(new PasswordSettingsValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
