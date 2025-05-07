using FluentValidation;
using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Senders;

namespace Krakenar.Core.Messages.Validators;

public class RecipientValidator : AbstractValidator<RecipientPayload>
{
  public RecipientValidator(SenderKind senderKind)
  {
    RuleFor(x => x.Type).IsInEnum();

    When(x => !string.IsNullOrWhiteSpace(x.Address), () => RuleFor(x => x.Address).EmailAddress());

    switch (senderKind)
    {
      case SenderKind.Email:
        RuleFor(x => x.PhoneNumber).Empty();
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.Address) || x.UserId.HasValue)
          .WithErrorCode(nameof(RecipientValidator))
          .WithMessage($"At least one of the following must be specified: {nameof(RecipientPayload.Address)}, {nameof(RecipientPayload.UserId)}.");
        break;
      case SenderKind.Phone:
        RuleFor(x => x.Address).Empty();
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.PhoneNumber) || x.UserId.HasValue)
          .WithErrorCode(nameof(RecipientValidator))
          .WithMessage($"At least one of the following must be specified: {nameof(RecipientPayload.PhoneNumber)}, {nameof(RecipientPayload.UserId)}.");
        break;
      default:
        throw new ArgumentException($"The sender kind '{senderKind}' is not supported.", nameof(senderKind));
    }
  }
}
