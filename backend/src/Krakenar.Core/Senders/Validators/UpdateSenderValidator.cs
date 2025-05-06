using FluentValidation;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Users.Validators;

namespace Krakenar.Core.Senders.Validators;

public class UpdateSenderValidator : AbstractValidator<UpdateSenderPayload>
{
  public UpdateSenderValidator(SenderProvider provider)
  {
    SenderKind kind = provider.GetSenderKind();
    switch (kind)
    {
      case SenderKind.Email:
        When(x => x.Email is not null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
        RuleFor(x => x.Phone).Null();
        break;
      case SenderKind.Phone:
        RuleFor(x => x.Email).Null();
        When(x => x.Phone is not null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));
        break;
    }

    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    switch (provider)
    {
      case SenderProvider.SendGrid:
        When(x => x.SendGrid is not null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
        RuleFor(x => x.Twilio).Null();
        break;
      case SenderProvider.Twilio:
        RuleFor(x => x.SendGrid).Null();
        When(x => x.Twilio is not null, () => RuleFor(x => x.Twilio!).SetValidator(new TwilioSettingsValidator()));
        break;
      default:
        throw new SenderProviderNotSupported(provider);
    }
  }
}
