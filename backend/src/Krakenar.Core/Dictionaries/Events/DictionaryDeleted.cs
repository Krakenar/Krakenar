using Logitar.EventSourcing;

namespace Krakenar.Core.Dictionaries.Events;

public record DictionaryDeleted : DomainEvent, IDeleteEvent;
