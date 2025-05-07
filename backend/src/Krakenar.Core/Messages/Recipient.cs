using Krakenar.Contracts.Messages;
using Krakenar.Core.Users;
using Logitar;

namespace Krakenar.Core.Messages;

public record Recipient
{
  public RecipientType Type { get; }

  public string? Address { get; }
  public string? DisplayName { get; }

  public string? PhoneNumber { get; }

  public UserId? UserId { get; }

  [JsonIgnore]
  public User? User { get; }

  [JsonConstructor]
  public Recipient(RecipientType type = RecipientType.To, string? address = null, string? displayName = null, string? phoneNumber = null, UserId? userId = null)
  {
    Type = type;
    Address = address?.CleanTrim();
    DisplayName = displayName?.CleanTrim();
    PhoneNumber = phoneNumber?.CleanTrim();
    UserId = userId;
    //new RecipientValidator().ValidateAndThrow(this); // TODO(fpion): validate
  }

  public Recipient(User user, RecipientType type = RecipientType.To)
    : this(type, user.Email?.Address, user.FullName, user.Phone?.FormatToE164(), user.Id)
  {
    User = user;
  }
}
