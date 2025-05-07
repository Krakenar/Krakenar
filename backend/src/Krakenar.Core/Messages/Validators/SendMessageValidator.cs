using FluentValidation;
using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Senders;

namespace Krakenar.Core.Messages.Validators;

public class SendMessageValidator : AbstractValidator<SendMessagePayload>
{
  public SendMessageValidator(SenderKind senderKind)
  {
    RuleFor(x => x.Sender).NotEmpty();
    RuleFor(x => x.Template).NotEmpty();

    RuleFor(x => x.Recipients).Must(r => r.Any(x => x.Type == RecipientType.To))
      .WithErrorCode("RecipientsValidator")
      .WithMessage($"'{{PropertyName}}' must contain at least one {nameof(RecipientType.To)} recipient.");
    RuleForEach(x => x.Recipients).SetValidator(new RecipientValidator(senderKind));

    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());

    RuleForEach(x => x.Variables).SetValidator(new VariableValidator());
  }
}
