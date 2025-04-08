using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Users;

public class UserNotFoundException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified user could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string User
  {
    get => (string)Data[nameof(User)]!;
    private set => Data[nameof(User)] = value;
  }
  public string PropertyName
  {
    get => (string?)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(User)] = User;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UserNotFoundException(RealmId? realmId, string user, string propertyName) : base(BuildMessage(realmId, user, propertyName))
  {
    RealmId = realmId?.ToGuid();
    User = user;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, string user, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(User), user)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
