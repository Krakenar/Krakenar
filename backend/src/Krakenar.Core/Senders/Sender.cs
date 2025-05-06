using Krakenar.Contracts.Senders;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders.Events;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Senders;

public class Sender : AggregateRoot
{
  private SenderUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null || _updated.Description is not null;

  public new SenderId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  public SenderKind Kind { get; private set; }
  public bool IsDefault { get; private set; }

  public Email? Email { get; private set; }
  public Phone? Phone { get; private set; }
  private DisplayName? _displayName = null;
  public DisplayName? DisplayName
  {
    get => _displayName;
    set
    {
      if (_displayName != value)
      {
        _displayName = value;
        _updated.DisplayName = new Change<DisplayName>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

  public SenderProvider Provider { get; private set; }
  private SenderSettings? _settings = null;
  public SenderSettings Settings => _settings ?? throw new InvalidOperationException("The sender has not been initialized.");

  public Sender() : base()
  {
  }

  public Sender(Email email, SenderSettings settings, bool isDefault = false, ActorId? actorId = null, SenderId? senderId = null)
    : this(email, phone: null, settings, isDefault, actorId, senderId)
  {
  }
  public Sender(Phone phone, SenderSettings settings, bool isDefault = false, ActorId? actorId = null, SenderId? senderId = null)
    : this(email: null, phone, settings, isDefault, actorId, senderId)
  {
  }
  private Sender(Email? email, Phone? phone, SenderSettings settings, bool isDefault = false, ActorId? actorId = null, SenderId? senderId = null)
    : base((senderId ?? SenderId.NewId()).StreamId)
  {
    if (email is not null)
    {
      Raise(new EmailSenderCreated(email, isDefault, settings.Provider), actorId);
    }
    else if (phone is not null)
    {
      Raise(new PhoneSenderCreated(phone, isDefault, settings.Provider), actorId);
    }
    else
    {
      throw new InvalidOperationException($"Either the '{nameof(email)}' or the '{nameof(phone)}' parameter must be provided.");
    }

    switch (settings.Provider)
    {
      case SenderProvider.SendGrid:
        SetSettings((SendGridSettings)settings, actorId);
        break;
      case SenderProvider.Twilio:
        SetSettings((TwilioSettings)settings, actorId);
        break;
      default:
        throw new SenderProviderNotSupported(settings.Provider);
    }
  }
  protected virtual void Handle(EmailSenderCreated @event)
  {
    Kind = SenderKind.Email;
    IsDefault = @event.IsDefault;

    Email = @event.Email;

    Provider = @event.Provider;
  }
  protected virtual void Handle(PhoneSenderCreated @event)
  {
    Kind = SenderKind.Phone;
    IsDefault = @event.IsDefault;

    Phone = @event.Phone;

    Provider = @event.Provider;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new SenderDeleted(), actorId);
    }
  }

  public void SetDefault(bool isDefault = true, ActorId? actorId = null)
  {
    if (IsDefault != isDefault)
    {
      Raise(new SenderSetDefault(isDefault), actorId);
    }
  }
  protected virtual void Handle(SenderSetDefault @event)
  {
    IsDefault = @event.IsDefault;
  }

  public void SetSettings(SendGridSettings settings, ActorId? actorId = null)
  {
    if (Provider != SenderProvider.SendGrid)
    {
      throw new SenderProviderMismatchException(this, settings.Provider);
    }

    if (_settings != settings)
    {
      Raise(new SendGridSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(SendGridSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(TwilioSettings settings, ActorId? actorId = null)
  {
    if (Provider != SenderProvider.Twilio)
    {
      throw new SenderProviderMismatchException(this, settings.Provider);
    }

    if (_settings != settings)
    {
      Raise(new TwilioSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(TwilioSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new SenderUpdated();
    }
  }
  protected virtual void Handle(SenderUpdated @event)
  {
    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }
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
        sender.Append(Email);
        break;
      case SenderKind.Phone:
        sender.Append(Phone);
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
