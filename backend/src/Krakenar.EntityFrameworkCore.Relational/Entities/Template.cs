using Krakenar.Contracts.Templates;
using Krakenar.Core.Templates;
using Krakenar.Core.Templates.Events;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar;
using Logitar.EventSourcing;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Template : Aggregate, ISegregatedEntity
{
  public int TemplateId { get; private set; }

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

  public string Subject { get; private set; } = string.Empty;
  public string ContentType { get; private set; } = string.Empty;
  public string ContentText { get; private set; } = string.Empty;

  public List<Message> Messages { get; private set; } = [];

  public Template(Realm? realm, TemplateCreated @event) : base(@event)
  {
    Realm = realm;
    RealmId = realm?.RealmId;
    RealmUid = realm?.Id;

    Id = new TemplateId(@event.StreamId).EntityId;

    UniqueName = @event.UniqueName.Value;

    Subject = @event.Subject.Value;
    SetContent(@event.Content);
  }

  private Template() : base()
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

  public void SetUniqueName(TemplateUniqueNameChanged @event)
  {
    Update(@event);

    UniqueName = @event.UniqueName.Value;
  }

  public void Update(TemplateUpdated @event)
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

    if (@event.Subject is not null)
    {
      Subject = @event.Subject.Value;
    }
    if (@event.Content is not null)
    {
      SetContent(@event.Content);
    }
  }

  private void SetContent(IContent content)
  {
    ContentType = content.Type;
    ContentText = content.Text;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
