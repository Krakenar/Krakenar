using Krakenar.Contracts.Senders;
using Krakenar.Core.Users;

namespace Krakenar.Core.Senders.Events;

public record PhoneSenderCreated : SenderCreated
{
  public Phone Phone { get; }

  public PhoneSenderCreated(Phone phone, bool isDefault, SenderProvider provider) : base(isDefault, provider)
  {
    Phone = phone;
  }
}
