using Logitar.EventSourcing;

namespace Krakenar.Core.Localization.Events;

public record LanguageSetDefault(bool IsDefault) : DomainEvent;
