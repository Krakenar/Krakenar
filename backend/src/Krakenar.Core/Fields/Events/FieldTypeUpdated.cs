using Logitar.EventSourcing;

namespace Krakenar.Core.Fields.Events;

public record FieldTypeUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }
}
