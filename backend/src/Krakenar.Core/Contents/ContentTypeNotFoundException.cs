using Krakenar.Contracts;
using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Contents;

public class ContentTypeNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified content type could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string ContentType
  {
    get => (string)Data[nameof(ContentType)]!;
    private set => Data[nameof(ContentType)] = value;
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
      error.Data[nameof(ContentType)] = ContentType;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ContentTypeNotFoundException(RealmId? realmId, string contentType, string propertyName) : base(BuildMessage(realmId, contentType, propertyName))
  {
    RealmId = realmId?.ToGuid();
    ContentType = contentType;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RealmId? realmId, string contentType, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realmId?.ToGuid(), "<null>")
    .AddData(nameof(ContentType), contentType)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
