using Krakenar.Core.Localization;
using Logitar.EventSourcing;

namespace Krakenar.Core.Dictionaries.Events;

public record DictionaryCreated(LanguageId LanguageId) : DomainEvent;
