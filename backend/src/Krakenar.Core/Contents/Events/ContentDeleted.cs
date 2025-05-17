using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentDeleted : DomainEvent, IDeleteEvent;
