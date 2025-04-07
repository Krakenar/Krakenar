using Logitar.EventSourcing;

namespace Krakenar.Core.Localization.Events;

public record LanguageLocaleChanged(Locale Locale) : DomainEvent;
