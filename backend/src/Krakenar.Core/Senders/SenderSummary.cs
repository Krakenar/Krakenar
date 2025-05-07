using Krakenar.Contracts.Senders;
using Krakenar.Core.Users;

namespace Krakenar.Core.Senders;

public record SenderSummary
{
  public SenderId Id { get; }
  public bool IsDefault { get; }
  public Email? Email { get; }
  public Phone? Phone { get; }
  public DisplayName? DisplayName { get; }
  public SenderProvider Provider { get; }

  public SenderSummary(SenderId id, bool isDefault, Email email, DisplayName? displayName, SenderProvider provider)
    : this(id, isDefault, email, phone: null, displayName, provider)
  {
  }

  [JsonConstructor]
  public SenderSummary(SenderId id, bool isDefault, Email? email, Phone? phone, DisplayName? displayName, SenderProvider provider)
  {
    Id = id;
    IsDefault = isDefault;
    Email = email;
    Phone = phone;
    DisplayName = displayName;
    Provider = provider;
  }

  public SenderSummary(Sender sender) : this(sender.Id, sender.IsDefault, sender.Email, sender.Phone, sender.DisplayName, sender.Provider)
  {
  }
}
