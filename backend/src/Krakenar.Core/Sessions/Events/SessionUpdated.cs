using Logitar.EventSourcing;

namespace Krakenar.Core.Sessions.Events;

public record SessionUpdated : DomainEvent
{
  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];
}
