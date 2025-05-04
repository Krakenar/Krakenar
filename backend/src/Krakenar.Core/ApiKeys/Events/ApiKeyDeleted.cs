using Logitar.EventSourcing;

namespace Krakenar.Core.ApiKeys.Events;

public record ApiKeyDeleted : DomainEvent, IDeleteEvent;
