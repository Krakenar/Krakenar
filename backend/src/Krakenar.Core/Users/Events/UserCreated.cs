using Krakenar.Core.Passwords;
using Logitar.EventSourcing;
using MediatR;

namespace Krakenar.Core.Users.Events;

public record UserCreated(UniqueName UniqueName, Password? Password) : DomainEvent, INotification;
