using Krakenar.Contracts.Senders;
using Logitar;

namespace Krakenar.Core.Senders;

public class SenderProviderMismatchException : Exception
{
  private const string ErrorMessage = "The specified sender provider was not expected.";

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
  public SenderProvider ExpectedProvider
  {
    get => (SenderProvider)Data[nameof(ExpectedProvider)]!;
    private set => Data[nameof(ExpectedProvider)] = value;
  }
  public SenderProvider ActualProvider
  {
    get => (SenderProvider)Data[nameof(ActualProvider)]!;
    private set => Data[nameof(ActualProvider)] = value;
  }

  public SenderProviderMismatchException(Sender sender, SenderProvider actualProvider) : base(BuildMessage(sender, actualProvider))
  {
    RealmId = sender.RealmId?.ToGuid();
    SenderId = sender.EntityId;
    ExpectedProvider = sender.Provider;
    ActualProvider = actualProvider;
  }

  private static string BuildMessage(Sender sender, SenderProvider actualProvider) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), sender.RealmId?.ToGuid())
    .AddData(nameof(SenderId), sender.EntityId)
    .AddData(nameof(ExpectedProvider), sender.Provider)
    .AddData(nameof(ActualProvider), actualProvider)
    .Build();
}
