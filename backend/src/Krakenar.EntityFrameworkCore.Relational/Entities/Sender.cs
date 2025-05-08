using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Senders.Settings;
using Krakenar.Core.Senders;
using Krakenar.Core.Senders.Events;
using Krakenar.Core.Users;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Sender : Aggregate, ISegregatedEntity
{
  public int SenderId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public SenderKind Kind { get; private set; }
  public bool IsDefault { get; private set; }

  public string? EmailAddress { get; private set; }
  public string? PhoneCountryCode { get; private set; }
  public string? PhoneNumber { get; private set; }
  public string? PhoneE164Formatted { get; private set; }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public SenderProvider Provider { get; private set; }
  public string? Settings { get; private set; }

  public List<Message> Messages { get; private set; } = [];

  public Sender(Realm? realm, EmailSenderCreated @event) : this(realm, (SenderCreated)@event)
  {
    Kind = SenderKind.Email;

    SetEmail(@event.Email);
  }
  public Sender(Realm? realm, PhoneSenderCreated @event) : this(realm, (SenderCreated)@event)
  {
    Kind = SenderKind.Phone;

    SetPhone(@event.Phone);
  }
  private Sender(Realm? realm, SenderCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
    RealmUid = realm?.Id;

    Id = new SenderId(@event.StreamId).EntityId;

    IsDefault = @event.IsDefault;

    Provider = @event.Provider;
  }

  private Sender() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Realm is not null)
    {
      actorIds.AddRange(Realm.GetActorIds());
    }
    return actorIds.ToList().AsReadOnly();
  }

  public void SetDefault(SenderSetDefault @event)
  {
    Update(@event);

    IsDefault = @event.IsDefault;
  }

  public void SetSettings(SendGridSettingsChanged @event)
  {
    Update(@event);

    SendGridSettings settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }
  public void SetSettings(TwilioSettingsChanged @event)
  {
    Update(@event);

    TwilioSettings settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }

  public void Update(SenderUpdated @event)
  {
    base.Update(@event);

    if (@event.Email is not null)
    {
      SetEmail(@event.Email);
    }
    if (@event.Phone is not null)
    {
      SetPhone(@event.Phone);
    }
    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }
  }

  private void SetEmail(Email email)
  {
    EmailAddress = email.Address;
  }
  private void SetPhone(Phone phone)
  {
    PhoneCountryCode = phone.CountryCode;
    PhoneNumber = phone.Number;
    PhoneE164Formatted = phone.FormatToE164();
  }

  public override string ToString()
  {
    StringBuilder sender = new();
    if (DisplayName is not null)
    {
      sender.Append(DisplayName).Append(" <");
    }
    switch (Kind)
    {
      case SenderKind.Email:
        sender.Append(EmailAddress);
        break;
      case SenderKind.Phone:
        sender.Append(PhoneNumber);
        break;
    }
    if (DisplayName is not null)
    {
      sender.Append('>');
    }
    sender.Append(" | ").Append(base.ToString());
    return sender.ToString();
  }
}
