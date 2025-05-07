using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Messages;

public class InvalidSmsMessageContentTypeException : BadRequestException
{
  private const string ErrorMessage = "A SMS message cannot be sent using the specified content type.";

  public string ContentType
  {
    get => (string)Data[nameof(ContentType)]!;
    private set => Data[nameof(ContentType)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(ContentType)] = ContentType;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidSmsMessageContentTypeException(string contentType, string? propertyName = null) : base(BuildMessage(contentType, propertyName))
  {
    ContentType = contentType;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string contentType, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ContentType), contentType)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
