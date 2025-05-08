using Logitar.EventSourcing;

namespace Krakenar.Core.Messages.Events;

public record MessageSucceeded(IReadOnlyDictionary<string, string> Results) : DomainEvent;
