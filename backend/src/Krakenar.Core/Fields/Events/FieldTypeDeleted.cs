using Logitar.EventSourcing;

namespace Krakenar.Core.Fields.Events;

public record FieldTypeDeleted : DomainEvent, IDeleteEvent;
