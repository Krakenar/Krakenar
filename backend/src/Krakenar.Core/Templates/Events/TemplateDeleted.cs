using Logitar.EventSourcing;

namespace Krakenar.Core.Templates.Events;

public record TemplateDeleted : DomainEvent, IDeleteEvent;
