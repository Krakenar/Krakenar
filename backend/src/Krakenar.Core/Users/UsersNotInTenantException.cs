using Krakenar.Core.Realms;

namespace Krakenar.Core.Users;

public class UsersNotInRealmException : Exception
{
  private const string ErrorMessage = "The specified users are not in the specified realm.";

  public Guid? ExpectedRealmId
  {
    get => (Guid?)Data[nameof(ExpectedRealmId)];
    private set => Data[nameof(ExpectedRealmId)] = value;
  }
  public IReadOnlyCollection<Guid> UserIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(UserIds)]!;
    private set => Data[nameof(UserIds)] = value;
  }

  public UsersNotInRealmException(RealmId? expectedRealm, IEnumerable<UserId> userIds) : base(BuildMessage(expectedRealm, userIds))
  {
    ExpectedRealmId = expectedRealm?.ToGuid();
    UserIds = userIds.Select(id => id.EntityId).ToArray();
  }

  private static string BuildMessage(RealmId? expectedRealm, IEnumerable<UserId> userIds)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.AppendLine(nameof(ExpectedRealmId)).Append(": ").AppendLine(expectedRealm?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(UserIds)).AppendLine(":");
    foreach (UserId userId in userIds)
    {
      message.AppendLine(" - ").AppendLine(userId.Value);
    }
    return message.ToString();
  }
}
