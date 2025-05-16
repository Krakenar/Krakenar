using FluentValidation;
using Krakenar.Contracts.Fields;

namespace Krakenar.Core.Fields.Validators;

public class UpdateFieldDefinitionValidator : AbstractValidator<UpdateFieldDefinitionPayload>
{
  public UpdateFieldDefinitionValidator(bool isInvariant)
  {
    if (isInvariant)
    {
      When(x => x.IsInvariant is not null, () => RuleFor(x => x.IsInvariant).Equal(true));
    }

    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).Identifier());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
    When(x => !string.IsNullOrWhiteSpace(x.Placeholder?.Value), () => RuleFor(x => x.Placeholder!.Value!).Placeholder());
  }
}
