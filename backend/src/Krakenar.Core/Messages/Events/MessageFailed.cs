using Logitar.EventSourcing;

namespace Krakenar.Core.Messages.Events;

public record MessageFailed(IReadOnlyDictionary<string, string> Results) : DomainEvent;
