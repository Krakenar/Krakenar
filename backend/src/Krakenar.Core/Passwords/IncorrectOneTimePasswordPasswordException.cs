using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Passwords;

public class IncorrectOneTimePasswordPasswordException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified password did not match the One-Time Password (OTP).";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid OneTimePasswordId
  {
    get => (Guid)Data[nameof(OneTimePasswordId)]!;
    private set => Data[nameof(OneTimePasswordId)] = value;
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
      error.Data[nameof(OneTimePasswordId)] = OneTimePasswordId;
      error.Data[nameof(AttemptedPassword)] = AttemptedPassword;
      return error;
    }
  }

  public IncorrectOneTimePasswordPasswordException(OneTimePassword oneTimePassword, string attemptedPassword)
    : base(BuildMessage(oneTimePassword, attemptedPassword))
  {
    RealmId = oneTimePassword.RealmId?.ToGuid();
    OneTimePasswordId = oneTimePassword.EntityId;
    AttemptedPassword = attemptedPassword;
  }

  private static string BuildMessage(OneTimePassword oneTimePassword, string attemptedPassword) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), oneTimePassword.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(OneTimePasswordId), oneTimePassword.EntityId)
    .AddData(nameof(AttemptedPassword), attemptedPassword)
    .Build();
}
