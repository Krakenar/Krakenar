using FluentValidation;
using Krakenar.Contracts.Fields;

namespace Krakenar.Core.Fields.Validators;

public class CreateOrReplaceFieldDefinitionValidator : AbstractValidator<CreateOrReplaceFieldDefinitionPayload>
{
  public CreateOrReplaceFieldDefinitionValidator(bool isInvariant)
  {
    RuleFor(x => x.FieldType).NotEmpty();

    if (isInvariant)
    {
      RuleFor(x => x.IsInvariant).Equal(true);
    }

    RuleFor(x => x.UniqueName).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
    When(x => !string.IsNullOrWhiteSpace(x.Placeholder), () => RuleFor(x => x.Placeholder!).Placeholder());
  }
}
