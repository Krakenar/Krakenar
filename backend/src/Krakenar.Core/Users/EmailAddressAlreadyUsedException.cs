using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Users;

public class EmailAddressAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified email address is already used.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid UserId
  {
    get => (Guid)Data[nameof(UserId)]!;
    private set => Data[nameof(UserId)] = value;
  }
  public IReadOnlyCollection<Guid> ConflictIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(ConflictIds)]!;
    private set => Data[nameof(ConflictIds)] = value;
  }
  public string EmailAddress
  {
    get => (string)Data[nameof(EmailAddress)]!;
    private set => Data[nameof(EmailAddress)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(UserId)] = UserId;
      error.Data[nameof(ConflictIds)] = ConflictIds;
      error.Data[nameof(EmailAddress)] = EmailAddress;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public EmailAddressAlreadyUsedException(User user, IEnumerable<UserId> conflictIds) : base(BuildMessage(user, conflictIds))
  {
    if (user.Email is null)
    {
      throw new ArgumentException($"The {nameof(user.Email)} is required.", nameof(user));
    }

    RealmId = user.RealmId?.ToGuid();
    UserId = user.EntityId;
    ConflictIds = conflictIds.Select(id => id.EntityId).ToList().AsReadOnly();
    EmailAddress = user.Email.Address;
    PropertyName = nameof(user.Email);
  }

  private static string BuildMessage(User user, IEnumerable<UserId> conflictIds)
  {
    if (user.Email is null)
    {
      throw new ArgumentException($"The {nameof(user.Email)} is required.", nameof(user));
    }

    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(RealmId)).AppendLine(user.RealmId?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(UserId)).Append(user.EntityId).AppendLine();
    message.Append(nameof(EmailAddress)).AppendLine(user.Email.Address);
    message.Append(nameof(PropertyName)).AppendLine(nameof(user.Email));
    message.Append(nameof(ConflictIds)).Append(':').AppendLine();
    foreach (UserId conflictId in conflictIds)
    {
      message.Append(" - ").Append(conflictId.EntityId).AppendLine();
    }
    return message.ToString();
  }
}
