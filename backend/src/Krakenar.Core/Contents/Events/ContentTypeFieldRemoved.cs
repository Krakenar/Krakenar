using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentTypeFieldRemoved(Guid FieldId) : DomainEvent;
