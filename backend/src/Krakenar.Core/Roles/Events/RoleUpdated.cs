using Logitar.EventSourcing;

namespace Krakenar.Core.Roles.Events;

public record RoleUpdated : DomainEvent
{
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];
}
