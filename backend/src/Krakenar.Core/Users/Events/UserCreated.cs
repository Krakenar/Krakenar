using Krakenar.Core.Passwords;
using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserCreated(UniqueName UniqueName, Password? Password) : DomainEvent;
