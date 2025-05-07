using Krakenar.Contracts.Senders.Settings;
using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Senders;

public record CreateOrReplaceSenderPayload
{
  public EmailPayload? Email { get; set; }
  public PhonePayload? Phone { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public SendGridSettings? SendGrid { get; set; }
  public TwilioSettings? Twilio { get; set; }
}
