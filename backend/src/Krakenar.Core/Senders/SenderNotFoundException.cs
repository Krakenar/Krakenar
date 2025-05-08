using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Senders;

public class SenderNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified sender could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string Sender
  {
    get => (string)Data[nameof(Sender)]!;
    private set => Data[nameof(Sender)] = value;
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
      error.Data[nameof(Sender)] = Sender;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public SenderNotFoundException(RealmId? realmId, string sender, string propertyName) : base(BuildMessage(realmId, sender, propertyName))
  {
    RealmId = realmId?.ToGuid();
    Sender = sender;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, string sender, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(Sender), sender)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
