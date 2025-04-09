using Logitar.EventSourcing;

namespace Krakenar.Core.Localization.Events;

public record LanguageCreated(bool IsDefault, Locale Locale) : DomainEvent;
