using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentTypeUniqueNameChanged(Identifier UniqueName) : DomainEvent;
