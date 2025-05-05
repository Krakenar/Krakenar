using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Templates;

namespace Krakenar.Core.Templates.Validators;

public class CreateOrReplaceTemplateValidator : AbstractValidator<CreateOrReplaceTemplatePayload>
{
  public CreateOrReplaceTemplateValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Subject).Subject();
    RuleFor(x => x.Content).SetValidator(new ContentValidator());
  }
}
