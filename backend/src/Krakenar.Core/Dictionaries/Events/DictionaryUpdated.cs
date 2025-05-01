using Logitar.EventSourcing;

namespace Krakenar.Core.Dictionaries.Events;

public record DictionaryUpdated : DomainEvent
{
  public Dictionary<Identifier, string?> Entries { get; set; } = [];
}
