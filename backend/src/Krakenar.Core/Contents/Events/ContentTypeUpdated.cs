using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentTypeUpdated : DomainEvent
{
  public bool? IsInvariant { get; set; }

  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }
}
