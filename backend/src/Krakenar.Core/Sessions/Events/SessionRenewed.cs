using Krakenar.Core.Passwords;
using Logitar.EventSourcing;

namespace Krakenar.Core.Sessions.Events;

public record SessionRenewed(Password Secret) : DomainEvent;
