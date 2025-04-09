using Krakenar.Core.Passwords;
using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public abstract record UserPasswordEvent(Password Password) : DomainEvent;
