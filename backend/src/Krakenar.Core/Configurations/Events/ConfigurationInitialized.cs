using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using MediatR;

namespace Krakenar.Core.Configurations.Events;

public record ConfigurationInitialized(/*Secret Secret,*/ UniqueNameSettings UniqueNameSettings, PasswordSettings PasswordSettings) : DomainEvent, INotification;
