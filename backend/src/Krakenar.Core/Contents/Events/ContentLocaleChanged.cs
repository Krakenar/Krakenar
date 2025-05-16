using Krakenar.Core.Localization;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentLocaleChanged(LanguageId? LanguageId, ContentLocale Locale) : DomainEvent;
