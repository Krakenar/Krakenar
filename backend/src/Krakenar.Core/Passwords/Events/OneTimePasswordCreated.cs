using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Passwords.Events;

public record OneTimePasswordCreated(Password Password, DateTime? ExpiresOn, int? MaximumAttempts, UserId? UserId) : DomainEvent;
