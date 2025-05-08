using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Messages;
using Krakenar.Core.Messages.Events;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using RecipientCore = Krakenar.Core.Messages.Recipient;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Message : Aggregate, ISegregatedEntity
{
  public int MessageId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string BodyType { get; private set; } = string.Empty;
  public string BodyText { get; private set; } = string.Empty;

  public int RecipientCount { get; private set; }
  public List<Recipient> Recipients { get; private set; } = [];

  public Sender? Sender { get; private set; }
  public int? SenderId { get; private set; }
  public Guid SenderUid { get; private set; }
  public bool SenderIsDefault { get; private set; }
  public string? SenderEmailAddress { get; private set; }
  public string? SenderPhoneCountryCode { get; private set; }
  public string? SenderPhoneNumber { get; private set; }
  public string? SenderPhoneExtension { get; private set; }
  public string? SenderPhoneE164Formatted { get; private set; }
  public string? SenderDisplayName { get; private set; }
  public SenderProvider SenderProvider { get; private set; }

  public Template? Template { get; private set; }
  public int? TemplateId { get; private set; }
  public Guid TemplateUid { get; private set; }
  public string TemplateUniqueName { get; private set; } = string.Empty;
  public string? TemplateDisplayName { get; private set; }

  public bool IgnoreUserLocale { get; private set; }
  public string? Locale { get; private set; }

  public string? Variables { get; private set; }

  public bool IsDemo { get; private set; }

  public MessageStatus Status { get; private set; }
  public string? Results { get; private set; }

  public Message(Realm? realm, Sender sender, Template template, IReadOnlyDictionary<string, User> users, MessageCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
    RealmUid = realm?.Id;

    Id = new MessageId(@event.StreamId).EntityId;

    Subject = @event.Subject.Value;
    BodyType = @event.Body.Type;
    BodyText = @event.Body.Text;

    foreach (RecipientCore recipient in @event.Recipients)
    {
      User? user = null;
      if (recipient.UserId.HasValue && !users.TryGetValue(recipient.UserId.Value.Value, out user))
      {
        throw new InvalidOperationException($"The user entity 'StreamId={recipient.UserId}' could not be found.");
      }
      Recipients.Add(new Recipient(this, recipient, user));
    }
    RecipientCount = Recipients.Count;

    SetSender(sender, @event);
    SetTemplate(template, @event);

    IgnoreUserLocale = @event.IgnoreUserLocale;
    Locale = @event.Locale?.Code;

    SetVariables(@event.Variables);

    IsDemo = @event.IsDemo;
  }
  private void SetSender(Sender sender, MessageCreated @event)
  {
    Sender = sender;
    SenderId = sender.SenderId;

    SenderUid = @event.Sender.Id.EntityId;
    SenderIsDefault = @event.Sender.IsDefault;
    SenderEmailAddress = @event.Sender.Email?.Address;
    if (@event.Sender.Phone is not null)
    {
      SenderPhoneCountryCode = @event.Sender.Phone.CountryCode;
      SenderPhoneNumber = @event.Sender.Phone.Number;
      SenderPhoneExtension = @event.Sender.Phone.Extension;
      SenderPhoneE164Formatted = @event.Sender.Phone.FormatToE164();
    }
    SenderDisplayName = @event.Sender.DisplayName?.Value;
    SenderProvider = @event.Sender.Provider;
  }
  private void SetTemplate(Template template, MessageCreated @event)
  {
    Template = template;
    TemplateId = template.TemplateId;

    TemplateUid = @event.Template.Id.EntityId;
    TemplateUniqueName = @event.Template.UniqueName.Value;
    TemplateDisplayName = @event.Template.DisplayName?.Value;
  }

  private Message()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = new(base.GetActorIds());
    if (Realm is not null)
    {
      actorIds.AddRange(Realm.GetActorIds());
    }
    foreach (Recipient recipient in Recipients)
    {
      if (recipient.User is not null)
      {
        actorIds.AddRange(recipient.User.GetActorIds());
      }
    }
    if (Sender is not null)
    {
      actorIds.AddRange(Sender.GetActorIds());
    }
    if (Template is not null)
    {
      actorIds.AddRange(Template.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Fail(MessageFailed @event)
  {
    Update(@event);

    Status = MessageStatus.Failed;
    SetResults(@event.Results);
  }

  public void Succeed(MessageSucceeded @event)
  {
    Update(@event);

    Status = MessageStatus.Succeeded;
    SetResults(@event.Results);
  }

  public Dictionary<string, string> GetVariables()
  {
    return (Variables is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(Variables)) ?? [];
  }
  private void SetVariables(IReadOnlyDictionary<string, string> variables)
  {
    Variables = variables.Count < 1 ? null : JsonSerializer.Serialize(variables);
  }

  public Dictionary<string, string> GetResults()
  {
    return (Results is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(Results)) ?? [];
  }
  private void SetResults(IReadOnlyDictionary<string, string> results)
  {
    Results = results.Count < 1 ? null : JsonSerializer.Serialize(results);
  }

  public override string ToString() => $"{Subject} | {base.ToString()}";
}
