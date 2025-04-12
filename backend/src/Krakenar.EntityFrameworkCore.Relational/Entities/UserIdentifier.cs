using Krakenar.Core.Users.Events;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class UserIdentifier : Identifier
{
  public int UserIdentifierId { get; private set; }

  public User? User { get; private set; }
  public int UserId { get; private set; }

  public UserIdentifier(User user, UserIdentifierChanged @event) : base(user.Realm, @event.Key.Value)
  {
    User = user;
    UserId = user.UserId;

    Update(@event);
  }

  private UserIdentifier() : base()
  {
  }

  public void Update(UserIdentifierChanged @event)
  {
    Value = @event.Value.Value;
  }

  public override bool Equals(object? obj) => obj is UserIdentifier identifier && identifier.UserIdentifierId == UserIdentifierId;
  public override int GetHashCode() => UserIdentifierId.GetHashCode();
  public override string ToString() => $"{GetType()} (UserIdentifierId={UserIdentifierId})";
}
