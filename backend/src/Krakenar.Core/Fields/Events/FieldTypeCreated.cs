using Krakenar.Contracts.Fields;
using Logitar.EventSourcing;

namespace Krakenar.Core.Fields.Events;

public record FieldTypeCreated(UniqueName UniqueName, DataType DataType) : DomainEvent;
