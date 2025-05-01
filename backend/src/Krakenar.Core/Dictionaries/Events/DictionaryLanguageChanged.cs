using Krakenar.Core.Localization;
using Logitar.EventSourcing;

namespace Krakenar.Core.Dictionaries.Events;

public record DictionaryLanguageChanged(LanguageId LanguageId) : DomainEvent;
