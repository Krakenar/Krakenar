using FluentValidation;
using Krakenar.Contracts.Fields.Settings;

namespace Krakenar.Core.Fields.Validators;

public class SelectSettingsValidator : AbstractValidator<SelectSettings>
{
  public SelectSettingsValidator()
  {
    RuleForEach(x => x.Options).SetValidator(new SelectOptionValidator());
  }
}
