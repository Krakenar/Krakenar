using FluentValidation;
using Krakenar.Contracts.Messages;

namespace Krakenar.Core.Messages.Validators;

public class SendMessageValidator : AbstractValidator<SendMessagePayload>
{
  public SendMessageValidator()
  {
    RuleFor(x => x.Sender).NotEmpty();
    RuleFor(x => x.Template).NotEmpty();
  }
}
