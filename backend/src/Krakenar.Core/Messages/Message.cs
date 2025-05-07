using Krakenar.Contracts.Messages;
using Krakenar.Core.Localization;
using Krakenar.Core.Messages.Events;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Messages;

public class Message : AggregateRoot
{
  public new MessageId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private Subject? _subject = null;
  public Subject Subject => _subject ?? throw new InvalidOperationException("The message has not been initialized.");
  private Content? _body = null;
  public Content Body => _body ?? throw new InvalidOperationException("The message has not been initialized.");

  private readonly List<Recipient> _recipients = [];
  public IReadOnlyCollection<Recipient> Recipients => _recipients.AsReadOnly();

  private SenderSummary? _sender = null;
  public SenderSummary Sender => _sender ?? throw new InvalidOperationException("The message has not been initialized.");
  private TemplateSummary? _template = null;
  public TemplateSummary Template => _template ?? throw new InvalidOperationException("The message has not been initialized.");

  public bool IgnoreUserLocale { get; private set; }
  public Locale? Locale { get; private set; }

  private readonly Dictionary<string, string> _variables = [];
  public IReadOnlyDictionary<string, string> Variables => _variables.AsReadOnly();

  public bool IsDemo { get; private set; }

  public MessageStatus Status { get; private set; }
  private readonly Dictionary<string, string> _results = [];
  public IReadOnlyDictionary<string, string> Results => _results.AsReadOnly();

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
    List<UserId> notInRealm = new(capacity: recipients.Count);
    int to = 0;
    foreach (Recipient recipient in recipients)
    {
      if (recipient.Type == RecipientType.To)
      {
        to++;
      }

      if (recipient.User is not null && recipient.User.RealmId != RealmId)
      {
        notInRealm.Add(recipient.User.Id);
      }
    }
    if (notInRealm.Count > 0)
    {
      throw new UsersNotInRealmException(RealmId, notInRealm);
    }
    else if (to == 0)
    {
      throw new ToRecipientMissingException(this, nameof(recipients));
    }

    if (sender.RealmId != RealmId)
    {
      throw new RealmMismatchException(RealmId, sender.RealmId, nameof(sender));
    }
    if (template.RealmId != RealmId)
    {
      throw new RealmMismatchException(RealmId, template.RealmId, nameof(template));
    }

    variables ??= new Dictionary<string, string>();
    Raise(new MessageCreated(subject, body, recipients, new SenderSummary(sender), new TemplateSummary(template), ignoreUserLocale, locale, variables, isDemo), actorId);
  }

  protected virtual void Handle(MessageCreated @event)
  {
    _subject = @event.Subject;
    _body = @event.Body;

    _recipients.AddRange(@event.Recipients);
    _sender = @event.Sender;
    _template = @event.Template;

    IgnoreUserLocale = @event.IgnoreUserLocale;
    Locale = @event.Locale;

    foreach (KeyValuePair<string, string> variable in @event.Variables)
    {
      _variables[variable.Key] = variable.Value;
    }

    IsDemo = @event.IsDemo;

    Status = MessageStatus.Unsent;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new MessageDeleted(), actorId);
    }
  }

  public void Fail(ActorId? actorId = null) => Fail(new Dictionary<string, string>(), actorId);
  public void Fail(IReadOnlyDictionary<string, string> resultData, ActorId? actorId = null)
  {
    if (Status == MessageStatus.Unsent)
    {
      Raise(new MessageFailed(resultData), actorId);
    }
  }
  protected virtual void Handle(MessageFailed @event)
  {
    Status = MessageStatus.Failed;

    foreach (KeyValuePair<string, string> result in @event.Results)
    {
      _results[result.Key] = result.Value;
    }
  }

  public void Succeed(ActorId? actorId = null) => Succeed(new Dictionary<string, string>(), actorId);
  public void Succeed(IReadOnlyDictionary<string, string> resultData, ActorId? actorId = null)
  {
    if (Status == MessageStatus.Unsent)
    {
      Raise(new MessageSucceeded(resultData), actorId);
    }
  }
  protected virtual void Handle(MessageSucceeded @event)
  {
    Status = MessageStatus.Succeeded;

    foreach (KeyValuePair<string, string> result in @event.Results)
    {
      _results[result.Key] = result.Value;
    }
  }

  public override string ToString() => $"{Subject} | {base.ToString()}";
}
