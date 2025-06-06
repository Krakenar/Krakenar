﻿using FluentValidation;
using Krakenar.Contracts.Templates;

namespace Krakenar.Core.Templates.Validators;

public class ContentValidator : AbstractValidator<IContent>
{
  public ContentValidator()
  {
    RuleFor(x => x.Type).MediaType();
    RuleFor(x => x.Text).NotEmpty();
  }
}
