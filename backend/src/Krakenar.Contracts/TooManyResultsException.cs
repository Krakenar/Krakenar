using Logitar;

namespace Krakenar.Contracts;

public class TooManyResultsException : ErrorException
{
  private const string ErrorMessage = "There are too many results.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public int ExpectedCount
  {
    get => (int)Data[nameof(ExpectedCount)]!;
    private set => Data[nameof(ExpectedCount)] = value;
  }
  public int ActualCount
  {
    get => (int)Data[nameof(ActualCount)]!;
    private set => Data[nameof(ActualCount)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(ExpectedCount)] = ExpectedCount;
      error.Data[nameof(ActualCount)] = ActualCount;
      return error;
    }
  }

  public TooManyResultsException(Type type, int expectedCount, int actualCount) : base(BuildMessage(type, expectedCount, actualCount))
  {
    TypeName = type.GetNamespaceQualifiedName();
    ExpectedCount = expectedCount;
    ActualCount = actualCount;
  }

  private static string BuildMessage(Type type, int expectedCount, int actualCount) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
    .AddData(nameof(ExpectedCount), expectedCount)
    .AddData(nameof(ActualCount), actualCount)
    .Build();
}

public class TooManyResultsException<T> : TooManyResultsException
{
  public TooManyResultsException(int expectedCount, int actualCount) : base(typeof(T), expectedCount, actualCount)
  {
  }

  public static TooManyResultsException<T> ExpectedSingle(int actualCount) => new(expectedCount: 1, actualCount);
}
