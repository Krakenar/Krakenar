using Logitar.EventSourcing;

namespace Krakenar.Core.Users.Events;

public record UserSignedIn : DomainEvent
{
  public UserSignedIn(DateTime occurredOn)
  {
    OccurredOn = occurredOn;
  }
}
