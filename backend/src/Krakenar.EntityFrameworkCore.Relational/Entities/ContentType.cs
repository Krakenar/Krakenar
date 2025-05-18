using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public class ContentType : Aggregate, ISegregatedEntity
{
  public int ContentTypeId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

  public Guid Id { get; private set; }

  public bool IsInvariant { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public int FieldCount { get; private set; }
  public List<FieldDefinition> FieldDefinitions { get; private set; } = [];

  public List<Content> Contents { get; private set; } = [];
  public List<ContentLocale> ContentLocales { get; private set; } = [];
  public List<FieldIndex> FieldIndex { get; private set; } = [];
  public List<PublishedContent> PublishedContents { get; private set; } = [];
  public List<UniqueIndex> UniqueIndex { get; private set; } = [];

  public ContentType(Realm? realm, ContentTypeCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
    RealmUid = realm?.Id;

    Id = new ContentTypeId(@event.StreamId).EntityId;

    IsInvariant = @event.IsInvariant;

    UniqueName = @event.UniqueName.Value;
  }

  private ContentType()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Realm is not null)
    {
      actorIds.AddRange(Realm.GetActorIds());
    }
    foreach (FieldDefinition field in FieldDefinitions)
    {
      actorIds.AddRange(field.GetActorIds());
    }
    return actorIds.ToList().AsReadOnly();
  }

  public FieldDefinition SetField(FieldType fieldType, ContentTypeFieldChanged @event)
  {
    Update(@event);

    FieldDefinition? fieldDefinition = FieldDefinitions.SingleOrDefault(f => f.Id == @event.Field.Id);
    if (fieldDefinition is null)
    {
      fieldDefinition = new FieldDefinition(this, fieldType, @event);
      FieldDefinitions.Add(fieldDefinition);
      FieldCount = FieldDefinitions.Count;
    }
    else
    {
      fieldDefinition.Update(@event);
    }
    return fieldDefinition;
  }

  public FieldDefinition? RemoveField(ContentTypeFieldRemoved @event)
  {
    Update(@event);

    FieldDefinition? fieldDefinition = FieldDefinitions.SingleOrDefault(f => f.Id == @event.FieldId);
    if (fieldDefinition is not null)
    {
      FieldDefinitions.Remove(fieldDefinition);
      FieldCount = FieldDefinitions.Count;

      foreach (FieldDefinition field in FieldDefinitions)
      {
        if (field.Order > fieldDefinition.Order)
        {
          field.Order--;
        }
      }
    }

    return fieldDefinition;
  }

  public void SetUniqueName(ContentTypeUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(ContentTypeUpdated @event)
  {
    base.Update(@event);

    if (@event.IsInvariant.HasValue)
    {
      IsInvariant = @event.IsInvariant.Value;
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

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
