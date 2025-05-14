using Logitar.EventSourcing;

namespace Krakenar.Core.Fields.Events;

public record FieldTypeUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
