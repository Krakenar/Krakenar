using FluentValidation;
using Krakenar.Contracts.Fields.Settings;

namespace Krakenar.Core.Fields.Validators;

public class SelectOptionValidator : AbstractValidator<ISelectOption>
{
  public SelectOptionValidator()
  {
    RuleFor(x => x.Text).NotEmpty();
  }
}
