namespace Krakenar;

internal record MessagingConfiguration
{
  public const string SectionKey = "Messaging";

  public RecipientConfiguration Recipient { get; set; } = new();
  public SendGridConfiguration SendGrid { get; set; } = new();
}

internal record RecipientConfiguration
{
  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
}

internal record SendGridConfiguration
{
  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string ApiKey { get; set; } = string.Empty;
}
