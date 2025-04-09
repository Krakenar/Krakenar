using Logitar.EventSourcing;

namespace Krakenar.Core.Realms.Events;

public record RealmDeleted : DomainEvent, IDeleteEvent;
