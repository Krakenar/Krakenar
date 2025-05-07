using Krakenar.Contracts.Messages;
using Logitar;

namespace Krakenar.Core.Messages;

public class ToRecipientMissingException : Exception
{
  private const string ErrorMessage = $"At least one {nameof(RecipientType.To)} recipient must be provided.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid MessageId
  {
    get => (Guid)Data[nameof(MessageId)]!;
    private set => Data[nameof(MessageId)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public ToRecipientMissingException(Message message, string? propertyName = null) : base(BuildMessage(message, propertyName))
  {
    RealmId = message.RealmId?.ToGuid();
    MessageId = message.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Message message, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), message.RealmId?.ToGuid())
    .AddData(nameof(MessageId), message.EntityId)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
