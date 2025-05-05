using Krakenar.Core.Realms;
using Krakenar.Core.Senders.Events;
using Logitar.EventSourcing;

namespace Krakenar.Core.Senders;

public class Sender : AggregateRoot
{
  private SenderUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null || _updated.Description is not null;

  public new SenderId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

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

  public Sender() : base()
  {
  }

  public Sender(ActorId? actorId = null, SenderId? senderId = null)
    : base((senderId ?? SenderId.NewId()).StreamId)
  {
    Raise(new SenderCreated(), actorId);
  }
  protected virtual void Handle(SenderCreated @event)
  {
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new SenderDeleted(), actorId);
    }
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

  public override string ToString() => $"{DisplayName?.Value} | {base.ToString()}";
}
