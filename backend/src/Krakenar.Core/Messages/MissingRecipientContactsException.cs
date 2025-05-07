using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Messages;

public class MissingRecipientContactsException : BadRequestException
{
  private const string ErrorMessage = "The specified recipients are missing an email address.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public IReadOnlyCollection<Guid> UserIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(UserIds)]!;
    private set => Data[nameof(UserIds)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(UserIds)] = UserIds;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public MissingRecipientContactsException(RealmId? realmId, IEnumerable<Guid> userIds, string propertyName) : base(BuildMessage(realmId, userIds, propertyName))
  {
    RealmId = realmId?.ToGuid();
    UserIds = userIds.ToArray();
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, IEnumerable<Guid> userIds, string propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(RealmId)).Append(": ").AppendLine(realmId?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.Append(nameof(UserIds)).AppendLine(":");
    foreach (Guid userId in userIds)
    {
      message.Append(" - ").Append(userId).AppendLine();
    }
    return message.ToString();
  }
}
