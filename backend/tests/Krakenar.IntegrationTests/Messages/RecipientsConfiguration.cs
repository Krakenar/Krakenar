using Krakenar.Contracts.Messages;

namespace Krakenar.Messages;

internal record RecipientsConfiguration
{
  public const string SectionKey = "Recipients";

  public List<EmailRecipient> Email { get; set; } = [];
  public List<PhoneRecipient> Phone { get; set; } = [];
}

internal record RecipientConfiguration
{
  public RecipientType Type { get; set; }
}

internal record EmailRecipient : RecipientConfiguration
{
  public string Address { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
}

internal record PhoneRecipient : RecipientConfiguration
{
  public string Number { get; set; } = string.Empty;
}
