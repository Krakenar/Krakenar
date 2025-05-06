using FluentValidation;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Senders.Settings;
using Krakenar.Core.Senders.Validators;

namespace Krakenar.Core.Senders.Settings;

public record TwilioSettings : SenderSettings, ITwilioSettings
{
  public override SenderProvider Provider => SenderProvider.Twilio;

  public string AccountSid { get; }
  public string AuthenticationToken { get; }

  [JsonConstructor]
  public TwilioSettings(string accountSid, string authenticationToken)
  {
    AccountSid = accountSid.Trim();
    AuthenticationToken = authenticationToken.Trim();
    new TwilioSettingsValidator().ValidateAndThrow(this);
  }

  public TwilioSettings(ITwilioSettings twilio) : this(twilio.AccountSid, twilio.AuthenticationToken)
  {
  }
}
