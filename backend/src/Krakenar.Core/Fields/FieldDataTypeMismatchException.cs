using Krakenar.Contracts.Fields;
using Logitar;

namespace Krakenar.Core.Fields;

public class FieldDataTypeMismatchException : ArgumentException
{
  private const string ErrorMessage = "The specified data type was not expected.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid FieldTypeId
  {
    get => (Guid)Data[nameof(FieldTypeId)]!;
    private set => Data[nameof(FieldTypeId)] = value;
  }
  public DataType ExpectedType
  {
    get => (DataType)Data[nameof(ExpectedType)]!;
    private set => Data[nameof(ExpectedType)] = value;
  }
  public DataType ActualType
  {
    get => (DataType)Data[nameof(ActualType)]!;
    private set => Data[nameof(ActualType)] = value;
  }

  public FieldDataTypeMismatchException(FieldType fieldType, DataType actualType, string paramName)
    : base(BuildMessage(fieldType, actualType), paramName)
  {
    RealmId = fieldType.RealmId?.ToGuid();
    FieldTypeId = fieldType.EntityId;
    ExpectedType = fieldType.DataType;
    ActualType = actualType;
  }

  private static string BuildMessage(FieldType fieldType, DataType actualType) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), fieldType.RealmId?.ToGuid())
    .AddData(nameof(FieldTypeId), fieldType.EntityId)
    .AddData(nameof(ExpectedType), fieldType.DataType)
    .AddData(nameof(ActualType), actualType)
    .Build();
}
