using Krakenar.Core.Localization;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using Logitar.EventSourcing;

namespace Krakenar.Core.Messages;

public class Message : AggregateRoot
{
  public new MessageId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public Message() : base()
  {
  }

  public Message(
    Subject subject,
    Content body,
    IReadOnlyCollection<Recipient> recipients,
    Sender sender,
    Template template,
    bool ignoreUserLocale = false,
    Locale? locale = null,
    IReadOnlyDictionary<string, string>? variables = null,
    bool isDemo = false,
    ActorId? actorId = null,
    MessageId? messageId = null) : base((messageId ?? MessageId.NewId()).StreamId)
  {
    // TODO(fpion): implement
  }

  // TODO(fpion): implement
}
