using Krakenar.Core.Localization;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using Logitar.EventSourcing;

namespace Krakenar.Core.Messages.Events;

public record MessageCreated(
  Subject Subject,
  Content Body,
  IReadOnlyCollection<Recipient> Recipients,
  SenderSummary Sender,
  TemplateSummary Template,
  bool IgnoreUserLocale,
  Locale? Locale,
  IReadOnlyDictionary<string, string> Variables,
  bool IsDemo) : DomainEvent;
