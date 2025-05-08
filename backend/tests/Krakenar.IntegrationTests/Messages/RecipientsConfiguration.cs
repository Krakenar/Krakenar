using Krakenar.Contracts.Messages;

namespace Krakenar.Messages;

internal record RecipientsConfiguration
{
  public const string SectionKey = "Recipients";

  public List<EmailRecipient> Email { get; set; } = [];
}

internal record EmailRecipient
{
  public RecipientType Type { get; set; }

  public string Address { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
}
