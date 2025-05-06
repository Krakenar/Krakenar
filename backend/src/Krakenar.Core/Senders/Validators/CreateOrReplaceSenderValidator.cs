using FluentValidation;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Users.Validators;

namespace Krakenar.Core.Senders.Validators;

public class CreateOrReplaceSenderValidator : AbstractValidator<CreateOrReplaceSenderPayload>
{
  public CreateOrReplaceSenderValidator()
  {
    When(x => x.Email is not null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone is not null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => x.SendGrid is not null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
    When(x => x.Twilio is not null, () => RuleFor(x => x.Twilio!).SetValidator(new TwilioSettingsValidator()));

    // TODO(fpion): only one setting
    // TODO(fpion): only one email/phone
    // TODO(fpion): email if SendGrid or phone if Twilio
    // TODO(fpion): cannot change a sender Kind, nor its Provider
  }
}
