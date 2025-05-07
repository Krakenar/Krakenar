using Krakenar.Contracts.Senders;
using Logitar.EventSourcing;

namespace Krakenar.Core.Senders.Events;

public abstract record SenderCreated(bool IsDefault, SenderProvider Provider) : DomainEvent;
