using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Sessions;

public class SessionNotFoundException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified session could not be found.";

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
      error.Data[nameof(SessionId)] = SessionId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public SessionNotFoundException(SessionId id, string propertyName) : base(BuildMessage(id, propertyName))
  {
    RealmId = id.RealmId?.ToGuid();
    SessionId = id.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(SessionId id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), id.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(SessionId), id.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
