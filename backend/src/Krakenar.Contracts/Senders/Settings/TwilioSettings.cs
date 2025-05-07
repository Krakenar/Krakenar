namespace Krakenar.Contracts.Senders.Settings;

public record TwilioSettings : ITwilioSettings
{
  public string AccountSid { get; set; }
  public string AuthenticationToken { get; set; }

  public TwilioSettings() : this(string.Empty, string.Empty)
  {
  }

  [JsonConstructor]
  public TwilioSettings(string accountSid, string authenticationToken)
  {
    AccountSid = accountSid;
    AuthenticationToken = authenticationToken;
  }

  public TwilioSettings(ITwilioSettings twilio) : this(twilio.AccountSid, twilio.AuthenticationToken)
  {
  }
}
