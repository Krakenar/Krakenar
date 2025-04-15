using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Users;
using Krakenar.Core.Validators;

namespace Krakenar.Core.Users.Validators;

public class CreateOrReplaceUserValidator : AbstractValidator<CreateOrReplaceUserPayload>
{
  public CreateOrReplaceUserValidator(IUniqueNameSettings uniqueNameSettings, IPasswordSettings passwordSettings, IAddressHelper addressHelper)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => x.Password is not null, () => RuleFor(x => x.Password!).SetValidator(new ChangePasswordValidator(passwordSettings)));

    When(x => x.Address is not null, () => RuleFor(x => x.Address!).SetValidator(new AddressValidator(addressHelper)));
    When(x => x.Email is not null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone is not null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.FirstName), () => RuleFor(x => x.FirstName!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.MiddleName), () => RuleFor(x => x.MiddleName!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.LastName), () => RuleFor(x => x.LastName!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.Nickname), () => RuleFor(x => x.Nickname!).PersonName());

    When(x => x.Birthdate.HasValue, () => RuleFor(x => x.Birthdate!.Value).Past());
    When(x => !string.IsNullOrWhiteSpace(x.Gender), () => RuleFor(x => x.Gender!).Gender());
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());
    When(x => !string.IsNullOrWhiteSpace(x.TimeZone), () => RuleFor(x => x.TimeZone!).TimeZone());

    When(x => !string.IsNullOrWhiteSpace(x.Picture), () => RuleFor(x => x.Picture!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Profile), () => RuleFor(x => x.Profile!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Website), () => RuleFor(x => x.Website!).Url());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeValidator());
  }
}
