using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class FieldDefinition
{
  public int FieldDefinitionId { get; private set; }

  public ContentType? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public Guid ContentTypeUid { get; private set; }

  public Guid Id { get; private set; }
  public int Order { get; set; }

  public FieldType? FieldType { get; private set; }
  public int FieldTypeId { get; private set; }
  public Guid FieldTypeUid { get; private set; }

  public bool IsInvariant { get; private set; }
  public bool IsRequired { get; private set; }
  public bool IsIndexed { get; private set; }
  public bool IsUnique { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }
  public string? Placeholder { get; private set; }

  public long Version { get; private set; }

  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }

  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public List<FieldIndex> FieldIndex { get; private set; } = [];
  public List<UniqueIndex> UniqueIndex { get; private set; } = [];

  public FieldDefinition(ContentType contentType, FieldType fieldType, ContentTypeFieldChanged @event)
  {
    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;
    ContentTypeUid = contentType.Id;

    Core.Fields.FieldDefinition field = @event.Field;
    Id = field.Id;
    Order = contentType.FieldDefinitions.Count;

    FieldType = fieldType;
    FieldTypeId = fieldType.FieldTypeId;
    FieldTypeUid = fieldType.Id;

    Update(@event);

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();
  }

  private FieldDefinition()
  {
  }

  public IReadOnlyCollection<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = new(capacity: 2);
    if (CreatedBy is not null)
    {
      actorIds.Add(new ActorId(CreatedBy));
    }
    if (UpdatedBy is not null)
    {
      actorIds.Add(new ActorId(UpdatedBy));
    }
    return actorIds.AsReadOnly();
  }

  public void Update(ContentTypeFieldChanged @event)
  {
    Core.Fields.FieldDefinition field = @event.Field;
    IsInvariant = field.IsInvariant;
    IsRequired = field.IsRequired;
    IsIndexed = field.IsIndexed;
    IsUnique = field.IsUnique;

    UniqueName = field.UniqueName.Value;
    DisplayName = field.DisplayName?.Value;
    Description = field.Description?.Value;
    Placeholder = field.Placeholder?.Value;

    Version = @event.Version;

    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is FieldDefinition field && field.ContentTypeId == ContentTypeId && field.Id == Id;
  public override int GetHashCode() => HashCode.Combine(ContentTypeId, Id);
  public override string ToString() => $"{DisplayName ?? UniqueName} (ContentTypeId={ContentTypeId}, Id={Id})";
}
