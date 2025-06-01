using FluentValidation;
using Krakenar.Contracts.Fields;

namespace Krakenar.Core.Fields.Validators;

public class SwitchFieldDefinitionsValidator : AbstractValidator<SwitchFieldDefinitionsPayload>
{
  public SwitchFieldDefinitionsValidator()
  {
    RuleFor(x => x.Fields).Must(BeAValidFieldSwitch)
      .WithErrorCode(nameof(SwitchFieldDefinitionsValidator))
      .WithMessage("'{PropertyName}' must contain exactly 2 different non-empty field IDs or unique names.");
  }

  private static bool BeAValidFieldSwitch(IEnumerable<string> fields) => fields.Where(field => !string.IsNullOrWhiteSpace(field)).Distinct().Count() == 2;
}
