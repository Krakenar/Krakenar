﻿using FluentValidation;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Contents.Validators;

public class CreateContentValidator : AbstractValidator<CreateContentPayload>
{
  public CreateContentValidator(IUniqueNameSettings uniqueNameSettings)
  {
    RuleFor(x => x.ContentType).NotEmpty();

    RuleFor(x => x.UniqueName).UniqueName(uniqueNameSettings);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
