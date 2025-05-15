using Krakenar.Core.Fields;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentTypeFieldChanged(FieldDefinition Field) : DomainEvent;
