using Logitar.EventSourcing;

namespace Krakenar.Core.Templates.Events;

public record TemplateUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
