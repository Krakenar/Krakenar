using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentTypeFieldSwitched(Guid SourceId, Guid DestinationId) : DomainEvent;
