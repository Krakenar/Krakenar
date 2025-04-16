using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Actors;

public class Actor
{
  public ActorType Type { get; set; }
  public Guid Id { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public Actor() : this(ActorType.System.ToString())
  {
  }

  public Actor(string displayName)
  {
    DisplayName = displayName;
  }

  public Actor(User user) : this(user.FullName ?? user.UniqueName)
  {
    Type = ActorType.User;
    Id = user.Id;

    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }

  public override bool Equals(object obj) => obj is Actor actor && actor.Type == Type && actor.Id == Id;
  public override int GetHashCode() => HashCode.Combine(Type, Id);
  public override string ToString()
  {
    StringBuilder actor = new();
    actor.Append(DisplayName.Trim());
    if (!string.IsNullOrWhiteSpace(EmailAddress))
    {
      actor.Append(" <").Append(EmailAddress.Trim()).Append('>');
    }
    actor.Append(" (").Append(Type).Append(".Id=").Append(Id).Append(')');
    return actor.ToString();
  }
}
