using Logitar.EventSourcing;

namespace Krakenar.Core.ApiKeys.Events;

public record ApiKeyUpdated : DomainEvent
{
  public DisplayName? Name { get; set; }
  public Change<Description>? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public Dictionary<Identifier, string?> CustomAttributes { get; set; } = [];
}
