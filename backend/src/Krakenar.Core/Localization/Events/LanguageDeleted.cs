using Logitar.EventSourcing;

namespace Krakenar.Core.Localization.Events;

public record LanguageDeleted : DomainEvent, IDeleteEvent;
