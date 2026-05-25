using FluentValidation;
using Krakenar.Contracts.Senders.Settings;

namespace Krakenar.Core.Senders.Validators;

public class SmtpProviderSettingsValidator : AbstractValidator<ISmtpProviderSettings>
{
  public SmtpProviderSettingsValidator()
  {
    RuleFor(x => x.Host).NotEmpty();
    RuleFor(x => x.Security).IsInEnum();
    RuleFor(x => x.Username).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();
  }
}
