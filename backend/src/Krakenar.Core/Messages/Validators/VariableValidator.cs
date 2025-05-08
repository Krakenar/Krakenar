using FluentValidation;
using Krakenar.Contracts.Messages;

namespace Krakenar.Core.Messages.Validators;

public class VariableValidator : AbstractValidator<Variable>
{
  public VariableValidator()
  {
    RuleFor(x => x.Key).Identifier();
    RuleFor(x => x.Value).NotEmpty();
  }
}
