﻿using FluentValidation;
using Krakenar.Contracts.Senders.Settings;

namespace Krakenar.Core.Senders.Validators;

public class TwilioSettingsValidator : AbstractValidator<ITwilioSettings>
{
  public TwilioSettingsValidator()
  {
    RuleFor(x => x.AccountSid).NotEmpty();
    RuleFor(x => x.AuthenticationToken).NotEmpty();
  }
}
