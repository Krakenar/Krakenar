using Krakenar.Core.Localization;
using Logitar.EventSourcing;
using MediatR;

namespace Krakenar.Core.Users.Events;

public record UserUpdated : DomainEvent, INotification
{
  public Change<Locale>? Locale { get; set; }
}
