using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserAddressChanged(Address? Address) : DomainEvent;
