using Krakenar.Contracts;
using Krakenar.Contracts.Senders;
using Logitar;

namespace Krakenar.Core.Senders;

public class CannotDeleteDefaultSenderException : BadRequestException
{
  private const string ErrorMessage = "The default sender cannot be deleted, unless there is no sender of the same kind.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid SenderId
  {
    get => (Guid)Data[nameof(SenderId)]!;
    private set => Data[nameof(SenderId)] = value;
  }
  public SenderKind Kind
  {
    get => (SenderKind)Data[nameof(Kind)]!;
    private set => Data[nameof(Kind)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(SenderId)] = SenderId;
      error.Data[nameof(Kind)] = Kind;
      return error;
    }
  }

  public CannotDeleteDefaultSenderException(Sender sender) : base(BuildMessage(sender))
  {
    RealmId = sender.RealmId?.ToGuid();
    SenderId = sender.EntityId;
    Kind = sender.Kind;
  }

  private static string BuildMessage(Sender sender) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), sender.RealmId?.ToGuid())
    .AddData(nameof(SenderId), sender.EntityId)
    .AddData(nameof(Kind), sender.Kind)
    .Build();
}
