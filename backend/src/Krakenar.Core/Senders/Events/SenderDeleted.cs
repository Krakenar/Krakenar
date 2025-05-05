using Logitar.EventSourcing;

namespace Krakenar.Core.Senders.Events;

public record SenderDeleted : DomainEvent, IDeleteEvent;
