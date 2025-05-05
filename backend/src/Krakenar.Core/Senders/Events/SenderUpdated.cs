using Logitar.EventSourcing;

namespace Krakenar.Core.Senders.Events;

public record SenderUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }
}
