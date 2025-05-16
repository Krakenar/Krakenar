using Krakenar.Core.Localization;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentLocaleRemoved(LanguageId LanguageId) : DomainEvent;
