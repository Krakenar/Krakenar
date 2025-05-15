using Krakenar.Core.Contents.Events;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Contents;

public class ContentType : AggregateRoot
{
  private ContentTypeUpdated _updated = new();
  private bool HasUpdates => _updated.IsInvariant is not null || _updated.DisplayName is not null || _updated.Description is not null;

  public new ContentTypeId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private bool _isInvariant = false;
  public bool IsInvariant
  {
    get => _isInvariant;
    set
    {
      if (_isInvariant != value)
      {
        _updated.IsInvariant = value;
        _isInvariant = value;
      }
    }
  }

  private Identifier? _uniqueName = null;
  public Identifier UniqueName => _uniqueName ?? throw new InvalidOperationException("The content type has not been initialized.");
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

  public ContentType() : base()
  {
  }

  public ContentType(Identifier uniqueName, bool isInvariant = false, ActorId? actorId = null, ContentTypeId? contentTypeId = null) : base(contentTypeId?.StreamId)
  {
    Raise(new ContentTypeCreated(isInvariant, uniqueName), actorId);
  }
  protected virtual void Handle(ContentTypeCreated @event)
  {
    _isInvariant = @event.IsInvariant;

    _uniqueName = @event.UniqueName;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ContentTypeDeleted(), actorId);
    }
  }

  public void SetUniqueName(Identifier uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new ContentTypeUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(ContentTypeUniqueNameChanged @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(ContentTypeUpdated @event)
  {
    if (@event.IsInvariant.HasValue)
    {
      _isInvariant = @event.IsInvariant.Value;
    }

    if (@event.DisplayName is not null)
    {
      _displayName = @event.DisplayName.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
