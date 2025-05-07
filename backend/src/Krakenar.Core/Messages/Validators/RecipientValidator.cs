using FluentValidation;
using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Users.Validators;

namespace Krakenar.Core.Messages.Validators;

public class RecipientValidator : AbstractValidator<RecipientPayload>
{
  public RecipientValidator(SenderKind senderKind)
  {
    RuleFor(x => x.Type).IsInEnum();

    When(x => x.Email is not null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone is not null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());

    switch (senderKind)
    {
      case SenderKind.Email:
        RuleFor(x => x.Phone).Null();
        RuleFor(x => x).Must(x => x.Email is not null || x.UserId.HasValue)
          .WithErrorCode(nameof(RecipientValidator))
          .WithMessage($"At least one of the following must be specified: {nameof(RecipientPayload.Email)}, {nameof(RecipientPayload.UserId)}.");
        break;
      case SenderKind.Phone:
        RuleFor(x => x.Email).Null();
        RuleFor(x => x).Must(x => x.Phone is not null || x.UserId.HasValue)
          .WithErrorCode(nameof(RecipientValidator))
          .WithMessage($"At least one of the following must be specified: {nameof(RecipientPayload.Phone)}, {nameof(RecipientPayload.UserId)}.");
        break;
      default:
        throw new ArgumentException($"The sender kind '{senderKind}' is not supported.", nameof(senderKind));
    }
  }
}
