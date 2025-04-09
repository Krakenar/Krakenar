using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Users;

public class IncorrectUserPasswordException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified password did not match the user.";

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
  public string AttemptedPassword
  {
    get => (string)Data[nameof(AttemptedPassword)]!;
    private set => Data[nameof(AttemptedPassword)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(UserId)] = UserId;
      error.Data[nameof(AttemptedPassword)] = AttemptedPassword;
      return error;
    }
  }

  public IncorrectUserPasswordException(User user, string attemptedPassword)
    : base(BuildMessage(user, attemptedPassword))
  {
    RealmId = user.RealmId?.ToGuid();
    UserId = user.EntityId;
    AttemptedPassword = attemptedPassword;
  }

  private static string BuildMessage(User user, string attemptedPassword) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), user.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(UserId), user.EntityId)
    .AddData(nameof(AttemptedPassword), attemptedPassword)
    .Build();
}
