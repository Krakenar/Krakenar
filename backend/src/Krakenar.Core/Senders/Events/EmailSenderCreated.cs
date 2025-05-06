using Krakenar.Contracts.Senders;
using Krakenar.Core.Users;

namespace Krakenar.Core.Senders.Events;

public record EmailSenderCreated : SenderCreated
{
  public Email Email { get; }

  public EmailSenderCreated(Email email, bool isDefault, SenderProvider provider) : base(isDefault, provider)
  {
    Email = email;
  }
}
