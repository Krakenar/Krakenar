using Krakenar.Core.Realms;
using Krakenar.Core.Templates.Events;
using Logitar.EventSourcing;

namespace Krakenar.Core.Templates;

public class Template : AggregateRoot
{
  private TemplateUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null || _updated.Description is not null;

  public new TemplateId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The template has not been initialized.");
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

  // TODO(fpion): Subject
  // TODO(fpion): Content

  public Template() : base()
  {
  }

  public Template(UniqueName uniqueName, ActorId? actorId = null, TemplateId? templateId = null)
    : base((templateId ?? TemplateId.NewId()).StreamId)
  {
    Raise(new TemplateCreated(uniqueName), actorId);
  }
  protected virtual void Handle(TemplateCreated @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new TemplateDeleted(), actorId);
    }
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new TemplateUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(TemplateUniqueNameChanged @event)
  {
    _uniqueName = @event.UniqueName;
  }

  public void Update(ActorId? actorId = null)
  {
    if (HasUpdates)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new TemplateUpdated();
    }
  }
  protected virtual void Handle(TemplateUpdated @event)
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

  public override string ToString() => $"{DisplayName?.Value ?? UniqueName.Value} | {base.ToString()}";
}
