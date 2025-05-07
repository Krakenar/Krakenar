using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Senders.Settings;
using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Senders;

public class Sender : Aggregate
{
  public Realm? Realm { get; set; }

  public SenderKind Kind { get; set; }
  public bool IsDefault { get; set; }

  public Email? Email { get; set; }
  public Phone? Phone { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public SenderProvider Provider { get; set; }
  public SendGridSettings? SendGrid { get; set; }
  public TwilioSettings? Twilio { get; set; }

  public override string ToString()
  {
    StringBuilder sender = new();
    if (DisplayName is not null)
    {
      sender.Append(DisplayName).Append(" <");
    }
    switch (Kind)
    {
      case SenderKind.Email:
        sender.Append(Email?.Address);
        break;
      case SenderKind.Phone:
        sender.Append(Phone?.E164Formatted);
        break;
    }
    if (DisplayName is not null)
    {
      sender.Append('>');
    }
    sender.Append(" | ").Append(base.ToString());
    return sender.ToString();
  }
}
