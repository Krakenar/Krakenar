namespace Krakenar.Senders;

internal record SenderConfiguration
{
  public const string SectionKey = "Senders";

  public SendGridConfiguration SendGrid { get; set; } = new();
  public TwilioConfiguration Twilio { get; set; } = new();
}

internal record SendGridConfiguration
{
  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string ApiKey { get; set; } = string.Empty;
}

internal record TwilioConfiguration
{
  public string PhoneNumber { get; set; } = string.Empty;
  public string AccountSid { get; set; } = string.Empty;
  public string AuthenticationToken { get; set; } = string.Empty;
}
