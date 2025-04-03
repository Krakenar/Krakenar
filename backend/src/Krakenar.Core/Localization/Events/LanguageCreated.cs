using Logitar.EventSourcing;
using MediatR;

namespace Krakenar.Core.Localization.Events;

public record LanguageCreated(bool IsDefault, Locale Locale) : DomainEvent, INotification;
