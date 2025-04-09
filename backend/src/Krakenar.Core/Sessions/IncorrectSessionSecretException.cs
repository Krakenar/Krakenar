using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Sessions;

public class IncorrectSessionSecretException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified secret did not match the session.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid SessionId
  {
    get => (Guid)Data[nameof(SessionId)]!;
    private set => Data[nameof(SessionId)] = value;
  }
  public string AttemptedSecret
  {
    get => (string)Data[nameof(AttemptedSecret)]!;
    private set => Data[nameof(AttemptedSecret)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(SessionId)] = SessionId;
      error.Data[nameof(AttemptedSecret)] = AttemptedSecret;
      return error;
    }
  }

  public IncorrectSessionSecretException(Session session, string attemptedSecret)
    : base(BuildMessage(session, attemptedSecret))
  {
    RealmId = session.RealmId?.ToGuid();
    SessionId = session.EntityId;
    AttemptedSecret = attemptedSecret;
  }

  private static string BuildMessage(Session session, string attemptedSecret) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), session.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(SessionId), session.EntityId)
    .AddData(nameof(AttemptedSecret), attemptedSecret)
    .Build();
}
