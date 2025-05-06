using Logitar.EventSourcing;

namespace Krakenar.Core.Senders.Events;

public record SenderSetDefault(bool IsDefault) : DomainEvent;
