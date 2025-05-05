using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Passwords;

public class OneTimePasswordIsExpiredException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified One-Time Password (OTP) is expired.";

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

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(OneTimePasswordId)] = OneTimePasswordId;
      return error;
    }
  }

  public OneTimePasswordIsExpiredException(OneTimePassword oneTimePassword) : base(BuildMessage(oneTimePassword))
  {
    RealmId = oneTimePassword.RealmId?.ToGuid();
    OneTimePasswordId = oneTimePassword.EntityId;
  }

  private static string BuildMessage(OneTimePassword oneTimePassword) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), oneTimePassword.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(OneTimePasswordId), oneTimePassword.EntityId)
    .Build();
}
