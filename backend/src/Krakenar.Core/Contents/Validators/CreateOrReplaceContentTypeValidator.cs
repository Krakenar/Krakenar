using FluentValidation;
using Krakenar.Contracts.Contents;

namespace Krakenar.Core.Contents.Validators;

public class CreateOrReplaceContentTypeValidator : AbstractValidator<CreateOrReplaceContentTypePayload>
{
  public CreateOrReplaceContentTypeValidator()
  {
    RuleFor(x => x.UniqueName).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
