using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Templates;

namespace Krakenar.Core.Templates.Validators;

public class UpdateTemplateValidator : AbstractValidator<UpdateTemplatePayload>
{
  public UpdateTemplateValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.Subject), () => RuleFor(x => x.Subject!).Subject());
    When(x => x.Content is not null, () => RuleFor(x => x.Content!).SetValidator(new ContentValidator()));
  }
}
