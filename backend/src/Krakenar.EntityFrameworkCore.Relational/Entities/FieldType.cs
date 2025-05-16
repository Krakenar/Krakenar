using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Events;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public class FieldType : Aggregate, ISegregatedEntity
{
  public int FieldTypeId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public DataType DataType { get; private set; }
  public string? Settings { get; private set; }

  public List<FieldDefinition> FieldDefinitions { get; private set; } = [];

  public FieldType(Realm? realm, FieldTypeCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
    RealmUid = realm?.Id;

    Id = new FieldTypeId(@event.StreamId).EntityId;

    UniqueName = @event.UniqueName.Value;

    DataType = @event.DataType;
  }

  private FieldType()
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

  public void SetSettings(FieldTypeBooleanSettingsChanged @event)
  {
    Update(@event);

    BooleanSettings settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }
  public void SetSettings(FieldTypeDateTimeSettingsChanged @event)
  {
    Update(@event);

    DateTimeSettings settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }
  public void SetSettings(FieldTypeNumberSettingsChanged @event)
  {
    Update(@event);

    NumberSettings settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }
  public void SetSettings(FieldTypeRelatedContentSettingsChanged @event)
  {
    Update(@event);

    RelatedContentSettings settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }
  public void SetSettings(FieldTypeRichTextSettingsChanged @event)
  {
    Update(@event);

    RichTextSettings settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }
  public void SetSettings(FieldTypeSelectSettingsChanged @event)
  {
    Update(@event);

    SelectSettings settings = new(
      @event.Settings.Options.Select(option => new SelectOption(option)).ToList(),
      @event.Settings.IsMultiple);
    Settings = JsonSerializer.Serialize(settings);
  }
  public void SetSettings(FieldTypeStringSettingsChanged @event)
  {
    Update(@event);

    StringSettings settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }
  public void SetSettings(FieldTypeTagsSettingsChanged @event)
  {
    Update(@event);

    TagsSettings settings = new(@event.Settings);
    Settings = JsonSerializer.Serialize(settings);
  }

  public void SetUniqueName(FieldTypeUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(FieldTypeUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName is not null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description is not null)
    {
      Description = @event.Description.Value?.Value;
    }
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
