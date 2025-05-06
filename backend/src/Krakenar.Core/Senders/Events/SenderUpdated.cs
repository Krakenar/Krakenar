using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Senders.Events;

public record SenderUpdated : DomainEvent
{
  public Email? Email { get; set; }
  public Phone? Phone { get; set; }
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }
}
