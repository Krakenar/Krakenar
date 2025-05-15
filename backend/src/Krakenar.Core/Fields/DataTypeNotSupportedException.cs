using Krakenar.Contracts.Fields;
using Logitar;

namespace Krakenar.Core.Fields;

public class DataTypeNotSupportedException : NotSupportedException
{
  private const string ErrorMessage = "The specified data type is not supported.";

  public DataType DataType
  {
    get => (DataType)Data[nameof(DataType)]!;
    private set => Data[nameof(DataType)] = value;
  }

  public DataTypeNotSupportedException(DataType dataType) : base(BuildMessage(dataType))
  {
    DataType = dataType;
  }

  private static string BuildMessage(DataType dataType) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(DataType), dataType)
    .Build();
}
