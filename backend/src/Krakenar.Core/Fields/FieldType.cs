using Krakenar.Contracts.Fields;
using Krakenar.Core.Fields.Events;
using Krakenar.Core.Fields.Settings;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;

namespace Krakenar.Core.Fields;

public class FieldType : AggregateRoot
{
  private FieldTypeUpdated _updated = new();
  private bool HasUpdates => _updated.DisplayName is not null || _updated.Description is not null;

  public new FieldTypeId Id => new(base.Id);
  public RealmId? RealmId => Id.RealmId;
  public Guid EntityId => Id.EntityId;

  private UniqueName? _uniqueName = null;
  public UniqueName UniqueName => _uniqueName ?? throw new InvalidOperationException("The field type has not been initialized.");
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

  public DataType DataType { get; private set; }
  private FieldTypeSettings? _settings = null;
  public FieldTypeSettings Settings => _settings ?? throw new InvalidOperationException("The field type has not been initialized.");

  public FieldType() : base()
  {
  }

  public FieldType(UniqueName uniqueName, FieldTypeSettings settings, ActorId? actorId = null, FieldTypeId? fieldTypeId = null)
    : base((fieldTypeId ?? FieldTypeId.NewId()).StreamId)
  {
    Raise(new FieldTypeCreated(uniqueName, settings.DataType), actorId);
    switch (settings.DataType)
    {
      case DataType.Boolean:
        SetSettings((BooleanSettings)settings, actorId);
        break;
      case DataType.DateTime:
        SetSettings((DateTimeSettings)settings, actorId);
        break;
      case DataType.Number:
        SetSettings((NumberSettings)settings, actorId);
        break;
      case DataType.RelatedContent:
        SetSettings((RelatedContentSettings)settings, actorId);
        break;
      case DataType.RichText:
        SetSettings((RichTextSettings)settings, actorId);
        break;
      case DataType.Select:
        SetSettings((SelectSettings)settings, actorId);
        break;
      case DataType.String:
        SetSettings((StringSettings)settings, actorId);
        break;
      case DataType.Tags:
        SetSettings((TagsSettings)settings, actorId);
        break;
      default:
        throw new DataTypeNotSupportedException(settings.DataType);
    }
  }
  protected virtual void Handle(FieldTypeCreated @event)
  {
    _uniqueName = @event.UniqueName;

    DataType = @event.DataType;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new FieldTypeDeleted(), actorId);
    }
  }

  public void SetSettings(BooleanSettings settings, ActorId? actorId = null)
  {
    if (DataType != settings.DataType)
    {
      throw new FieldDataTypeMismatchException(this, settings.DataType, nameof(settings));
    }

    if (_settings is null || !_settings.Equals(settings))
    {
      Raise(new FieldTypeBooleanSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(FieldTypeBooleanSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(DateTimeSettings settings, ActorId? actorId = null)
  {
    if (DataType != settings.DataType)
    {
      throw new FieldDataTypeMismatchException(this, settings.DataType, nameof(settings));
    }

    if (_settings is null || !_settings.Equals(settings))
    {
      Raise(new FieldTypeDateTimeSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(FieldTypeDateTimeSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(NumberSettings settings, ActorId? actorId = null)
  {
    if (DataType != settings.DataType)
    {
      throw new FieldDataTypeMismatchException(this, settings.DataType, nameof(settings));
    }

    if (_settings is null || !_settings.Equals(settings))
    {
      Raise(new FieldTypeNumberSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(FieldTypeNumberSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(RelatedContentSettings settings, ActorId? actorId = null)
  {
    if (DataType != settings.DataType)
    {
      throw new FieldDataTypeMismatchException(this, settings.DataType, nameof(settings));
    }

    if (_settings is null || !_settings.Equals(settings))
    {
      Raise(new FieldTypeRelatedContentSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(FieldTypeRelatedContentSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(RichTextSettings settings, ActorId? actorId = null)
  {
    if (DataType != settings.DataType)
    {
      throw new FieldDataTypeMismatchException(this, settings.DataType, nameof(settings));
    }

    if (_settings is null || !_settings.Equals(settings))
    {
      Raise(new FieldTypeRichTextSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(FieldTypeRichTextSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(SelectSettings settings, ActorId? actorId = null)
  {
    if (DataType != settings.DataType)
    {
      throw new FieldDataTypeMismatchException(this, settings.DataType, nameof(settings));
    }

    if (_settings is null || !_settings.Equals(settings))
    {
      Raise(new FieldTypeSelectSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(FieldTypeSelectSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(StringSettings settings, ActorId? actorId = null)
  {
    if (DataType != settings.DataType)
    {
      throw new FieldDataTypeMismatchException(this, settings.DataType, nameof(settings));
    }

    if (_settings is null || !_settings.Equals(settings))
    {
      Raise(new FieldTypeStringSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(FieldTypeStringSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetSettings(TagsSettings settings, ActorId? actorId = null)
  {
    if (DataType != settings.DataType)
    {
      throw new FieldDataTypeMismatchException(this, settings.DataType, nameof(settings));
    }

    if (_settings is null || !_settings.Equals(settings))
    {
      Raise(new FieldTypeTagsSettingsChanged(settings), actorId);
    }
  }
  protected virtual void Handle(FieldTypeTagsSettingsChanged @event)
  {
    _settings = @event.Settings;
  }

  public void SetUniqueName(UniqueName uniqueName, ActorId? actorId = null)
  {
    if (_uniqueName != uniqueName)
    {
      Raise(new FieldTypeUniqueNameChanged(uniqueName), actorId);
    }
  }
  protected virtual void Handle(FieldTypeUniqueNameChanged @event)
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
  protected virtual void Handle(FieldTypeUpdated @event)
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
