using FluentValidation;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Contents.Validators;

public class CreateContentValidator : AbstractValidator<CreateContentPayload>
{
  public CreateContentValidator(bool isInvariant, IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.ContentType).NotEmpty();

    if (isInvariant)
    {
      RuleFor(x => x.Language).Empty();
    }
    else
    {
      RuleFor(x => x.Language).NotEmpty();
    }

    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
