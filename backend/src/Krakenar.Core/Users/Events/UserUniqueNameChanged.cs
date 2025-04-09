using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserUniqueNameChanged(UniqueName UniqueName) : DomainEvent;
