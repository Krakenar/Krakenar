using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Messages;

public record RecipientPayload
{
  public RecipientType Type { get; set; }

  public EmailPayload? Email { get; set; }
  public PhonePayload? Phone { get; set; }
  public string? DisplayName { get; set; }

  public Guid? UserId { get; set; }

  public RecipientPayload()
  {
  }

  public RecipientPayload(EmailPayload email, string? displayName = null, RecipientType type = RecipientType.To)
  {
    Email = email;
    DisplayName = displayName;
    Type = type;
  }

  public RecipientPayload(PhonePayload phone, RecipientType type = RecipientType.To)
  {
    Phone = phone;
    Type = type;
  }

  public RecipientPayload(Guid userId, RecipientType type = RecipientType.To)
  {
    Type = type;

    UserId = userId;
  }
}
