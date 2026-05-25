using Krakenar.Core.Senders.Settings;
using Logitar.EventSourcing;

namespace Krakenar.Core.Senders.Events;

public record SmtpProviderSettingsChanged(SmtpProviderSettings Settings) : DomainEvent;
