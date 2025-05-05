using Logitar.EventSourcing;

namespace Krakenar.Core.Passwords.Events;

public record OneTimePasswordUpdated : DomainEvent
{
  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];
}
