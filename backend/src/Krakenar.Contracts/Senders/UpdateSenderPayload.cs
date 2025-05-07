using Krakenar.Contracts.Senders.Settings;
using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Senders;

public record UpdateSenderPayload
{
  public EmailPayload? Email { get; set; }
  public PhonePayload? Phone { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public SendGridSettings? SendGrid { get; set; }
  public TwilioSettings? Twilio { get; set; }
}
