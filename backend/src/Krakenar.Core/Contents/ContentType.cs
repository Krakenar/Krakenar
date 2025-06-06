﻿using FluentValidation;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Fields;
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

  private readonly Dictionary<Guid, int> _fieldIdIndex = [];
  private readonly Dictionary<string, int> _fieldNameIndex = [];
  private readonly List<FieldDefinition> _fields = [];
  public IReadOnlyCollection<FieldDefinition> Fields => _fields.AsReadOnly();

  public ContentType() : base()
  {
  }

  public ContentType(Identifier uniqueName, bool isInvariant = false, ActorId? actorId = null, ContentTypeId? contentTypeId = null)
    : base((contentTypeId ?? ContentTypeId.NewId()).StreamId)
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

  public FieldDefinition FindField(Guid id) => TryGetField(id) ?? throw new InvalidOperationException($"The field 'Id={id}' could not be found.");
  public FieldDefinition FindField(Identifier uniqueName) => TryGetField(uniqueName) ?? throw new InvalidOperationException($"The field 'UniqueName={uniqueName}' could not be found.");

  public bool HasField(Guid id) => TryGetField(id) is not null;
  public bool HasField(Identifier uniqueName) => TryGetField(uniqueName) is not null;

  public bool RemoveField(Guid id, ActorId? actorId = null)
  {
    if (!HasField(id))
    {
      return false;
    }

    Raise(new ContentTypeFieldRemoved(id), actorId);
    return true;
  }
  protected virtual void Handle(ContentTypeFieldRemoved @event)
  {
    int index = _fieldIdIndex[@event.FieldId];
    _fields.RemoveAt(index);

    foreach (KeyValuePair<Guid, int> pair in _fieldIdIndex)
    {
      if (pair.Value == index)
      {
        _fieldIdIndex.Remove(pair.Key);
      }
      else if (pair.Value > index)
      {
        _fieldIdIndex[pair.Key] = pair.Value - 1;
      }
    }

    foreach (KeyValuePair<string, int> pair in _fieldNameIndex)
    {
      if (pair.Value == index)
      {
        _fieldNameIndex.Remove(pair.Key);
      }
      else if (pair.Value > index)
      {
        _fieldNameIndex[pair.Key] = pair.Value - 1;
      }
    }
  }

  public FieldDefinition? ResolveField(string idOrUniqueName)
  {
    FieldDefinition? field = null;

    if (Guid.TryParse(idOrUniqueName, out Guid id))
    {
      field = TryGetField(id);
      if (field is not null)
      {
        return field;
      }
    }

    try
    {
      Identifier uniqueName = new(idOrUniqueName);
      field = TryGetField(uniqueName);
    }
    catch (ValidationException)
    {
    }

    return field;
  }

  public void SetField(FieldDefinition field, ActorId? actorId = null)
  {
    if (IsInvariant && !field.IsInvariant)
    {
      throw new ArgumentException("The field definition must be invariant when the content type is invariant.", nameof(field));
    }

    FieldDefinition? existingField = TryGetField(field.UniqueName);
    if (existingField is not null && existingField.Id != field.Id)
    {
      throw new FieldUniqueNameAlreadyUsedException(this, field, existingField.Id);
    }

    existingField = TryGetField(field.Id);
    if (existingField is null || existingField != field)
    {
      Raise(new ContentTypeFieldChanged(field), actorId);
    }
  }
  protected virtual void Handle(ContentTypeFieldChanged @event)
  {
    FieldDefinition field = @event.Field;
    string uniqueNameNormalized = Normalize(field.UniqueName);
    bool found = _fieldIdIndex.TryGetValue(field.Id, out int index);
    if (found)
    {
      _fields[index] = field;

      string uniqueName = _fieldNameIndex.Single(x => x.Value == index).Key;
      if (uniqueName != uniqueNameNormalized)
      {
        _fieldNameIndex.Remove(uniqueName);
        _fieldNameIndex[uniqueNameNormalized] = index;
      }
    }
    else
    {
      index = _fields.Count;
      _fields.Add(field);
      _fieldIdIndex[field.Id] = index;
      _fieldNameIndex[uniqueNameNormalized] = index;
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

  public void SwitchFields(FieldDefinition source, FieldDefinition destination, ActorId? actorId = null) => SwitchFields(source.Id, destination.Id, actorId);
  public void SwitchFields(Guid sourceId, Guid destinationId, ActorId? actorId = null)
  {
    if (sourceId != destinationId)
    {
      if (!HasField(sourceId))
      {
        throw new ArgumentException($"The field 'Id={sourceId}' was not found on content type '{this}'.", nameof(sourceId));
      }
      if (!HasField(destinationId))
      {
        throw new ArgumentException($"The field 'Id={destinationId}' was not found on content type '{this}'.", nameof(destinationId));
      }
      Raise(new ContentTypeFieldSwitched(sourceId, destinationId), actorId);
    }
  }
  protected virtual void Handle(ContentTypeFieldSwitched @event)
  {
    int sourceIndex = _fieldIdIndex[@event.SourceId];
    FieldDefinition source = _fields[sourceIndex];

    int destinationIndex = _fieldIdIndex[@event.DestinationId];
    FieldDefinition destination = _fields[destinationIndex];

    _fields[sourceIndex] = destination;
    _fields[destinationIndex] = source;

    _fieldIdIndex[source.Id] = destinationIndex;
    _fieldIdIndex[destination.Id] = sourceIndex;

    _fieldNameIndex[Normalize(source.UniqueName)] = destinationIndex;
    _fieldNameIndex[Normalize(destination.UniqueName)] = sourceIndex;
  }

  public FieldDefinition? TryGetField(Guid id) => _fieldIdIndex.TryGetValue(id, out int index) ? _fields[index] : null;
  public FieldDefinition? TryGetField(Identifier uniqueName) => _fieldNameIndex.TryGetValue(Normalize(uniqueName), out int index) ? _fields[index] : null;

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

  private static string Normalize(Identifier identifier) => identifier.Value.ToUpperInvariant();
}
