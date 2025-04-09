using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserPhoneChanged(Phone? Phone) : DomainEvent;
