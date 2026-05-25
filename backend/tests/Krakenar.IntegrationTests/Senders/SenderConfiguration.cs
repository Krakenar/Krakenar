using Krakenar.Contracts.Senders.Settings;

namespace Krakenar.Senders;

internal record SenderConfiguration
{
  public const string SectionKey = "Senders";

  public SendGridConfiguration SendGrid { get; set; } = new();
  public SmtpProviderConfiguration SmtpProvider { get; set; } = new();
  public TwilioConfiguration Twilio { get; set; } = new();
}

internal record SendGridConfiguration
{
  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string ApiKey { get; set; } = string.Empty;
}

internal record SmtpProviderConfiguration
{
  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string Host { get; set; } = string.Empty;
  public ushort Port { get; set; }
  public SmtpSecurityMode Security { get; set; }
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}

internal record TwilioConfiguration
{
  public string PhoneNumber { get; set; } = string.Empty;
  public string AccountSid { get; set; } = string.Empty;
  public string AuthenticationToken { get; set; } = string.Empty;
}
