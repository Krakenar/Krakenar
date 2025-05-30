﻿using FluentValidation;
using Krakenar.Contracts.Senders.Settings;

namespace Krakenar.Core.Senders.Validators;

public class SendGridSettingsValidator : AbstractValidator<ISendGridSettings>
{
  public SendGridSettingsValidator()
  {
    RuleFor(x => x.ApiKey).NotEmpty();
  }
}
