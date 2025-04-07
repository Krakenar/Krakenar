using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserUpdated : DomainEvent
{
  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];
}
