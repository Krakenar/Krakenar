using FluentValidation;
using Krakenar.Contracts.Messages;

namespace Krakenar.Core.Messages.Validators;

public class SendMessageValidator : AbstractValidator<SendMessagePayload>
{
  public SendMessageValidator()
  {
    RuleFor(x => x.Sender).NotEmpty();
    RuleFor(x => x.Template).NotEmpty();

    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());

    RuleForEach(x => x.Variables).SetValidator(new VariableValidator());
  }
}
