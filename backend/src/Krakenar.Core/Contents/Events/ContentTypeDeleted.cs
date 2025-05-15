using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentTypeDeleted : DomainEvent, IDeleteEvent;
