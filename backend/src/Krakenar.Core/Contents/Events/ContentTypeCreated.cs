using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentTypeCreated(bool IsInvariant, Identifier UniqueName) : DomainEvent;
