using Logitar.EventSourcing;

namespace Krakenar.Core.Passwords.Events;

public record OneTimePasswordValidationSucceeded : DomainEvent;
