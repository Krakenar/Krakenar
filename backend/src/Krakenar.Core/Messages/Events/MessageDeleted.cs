using Logitar.EventSourcing;

namespace Krakenar.Core.Messages.Events;

public record MessageDeleted : DomainEvent, IDeleteEvent;
