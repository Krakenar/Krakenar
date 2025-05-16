using Logitar.EventSourcing;

namespace Krakenar.Core.Contents.Events;

public record ContentCreated(ContentTypeId ContentTypeId, ContentLocale Invariant) : DomainEvent;
