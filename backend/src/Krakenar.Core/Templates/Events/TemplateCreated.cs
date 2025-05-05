using Logitar.EventSourcing;

namespace Krakenar.Core.Templates.Events;

public record TemplateCreated(UniqueName UniqueName, Subject Subject, Content Content) : DomainEvent;
