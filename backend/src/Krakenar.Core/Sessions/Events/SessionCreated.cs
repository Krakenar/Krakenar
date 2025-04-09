using Krakenar.Core.Passwords;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Sessions.Events;

public record SessionCreated(UserId UserId, Password? Secret) : DomainEvent;
