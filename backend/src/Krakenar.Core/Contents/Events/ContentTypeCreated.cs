using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentTypeCreated(bool IsInvariant, UniqueName UniqueName) : DomainEvent;
