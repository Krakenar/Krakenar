using Krakenar.Contracts.Actors;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class Actor
{
  public int ActorId { get; private set; }
  public string Key { get; private set; } = string.Empty;

  public int? RealmId { get; private set; }
  public Realm? Realm { get; private set; }

  public ActorType Type { get; private set; }
  public Guid Id { get; private set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; private set; } = string.Empty;
  public string? EmailAddress { get; private set; }
  public string? PictureUrl { get; private set; }

  public Actor(User user)
  {
    Key = user.StreamId;

    RealmId = user.RealmId;
    Realm = user.Realm;

    Type = ActorType.User;
    Id = user.Id;

    Update(user);
  }

  private Actor()
  {
  }

  public void Update(User user)
  {
    if (Type != ActorType.User)
    {
      throw new ArgumentException($"The actor ({this}) has the type '{Type}'. It cannot be updated from the user '{user}'.", nameof(user));
    }

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.EmailAddress;
    PictureUrl = user.Picture;
  }

  public override bool Equals(object? obj) => obj is Actor actor && actor.Key == Key;
  public override int GetHashCode() => Key.GetHashCode();
  public override string ToString() => $"{GetType()} (Key={Key})";
}
