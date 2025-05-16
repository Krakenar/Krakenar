using FluentValidation;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Contents.Validators;

public class UpdateContentLocaleValidator : AbstractValidator<UpdateContentLocalePayload>
{
  public UpdateContentLocaleValidator(IUniqueNameSettings uniqueNameSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(uniqueNameSettings));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
