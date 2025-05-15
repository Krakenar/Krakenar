using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentTypeUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
