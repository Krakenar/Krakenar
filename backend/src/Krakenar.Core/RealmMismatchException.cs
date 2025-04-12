using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core;

public class RealmMismatchException : ArgumentException
{
  private const string ErrorMessage = "The specified realm was not expected.";

  public Guid? ExpectedRealmId
  {
    get => (Guid?)Data[nameof(ExpectedRealmId)];
    private set => Data[nameof(ExpectedRealmId)] = value;
  }
  public Guid? ActualRealmId
  {
    get => (Guid?)Data[nameof(ActualRealmId)];
    private set => Data[nameof(ActualRealmId)] = value;
  }

  public RealmMismatchException(RealmId? expectedRealm, RealmId? actualRealm, string paramName)
    : this(expectedRealm?.ToGuid(), actualRealm?.ToGuid(), paramName)
  {
  }
  public RealmMismatchException(Guid? expectedRealm, Guid? actualRealm, string paramName)
    : base(BuildMessage(expectedRealm, actualRealm), paramName)
  {
    ExpectedRealmId = expectedRealm;
    ActualRealmId = actualRealm;
  }

  private static string BuildMessage(Guid? expectedRealm, Guid? actualRealm) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ExpectedRealmId), expectedRealm, "<null>")
    .AddData(nameof(ActualRealmId), actualRealm, "<null>")
    .Build();
}
