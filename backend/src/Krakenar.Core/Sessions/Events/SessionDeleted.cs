using Logitar.EventSourcing;

namespace Krakenar.Core.Sessions.Events;

public record SessionDeleted : DomainEvent, IDeleteEvent;
