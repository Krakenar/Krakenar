namespace Krakenar.Contracts.Senders.Settings;

public interface ITwilioSettings
{
  string AccountSid { get; }
  string AuthenticationToken { get; }
}
