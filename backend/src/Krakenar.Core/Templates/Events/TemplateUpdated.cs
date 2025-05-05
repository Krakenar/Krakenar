using Logitar.EventSourcing;

namespace Krakenar.Core.Templates.Events;

public record TemplateUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Subject? Subject { get; set; }
  public Content? Content { get; set; }
}
