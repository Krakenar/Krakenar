using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.ApiKeys;

public class ApiKeyNotFoundException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified API key could not be found.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid ApiKeyId
  {
    get => (Guid)Data[nameof(ApiKeyId)]!;
    private set => Data[nameof(ApiKeyId)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(ApiKeyId)] = ApiKeyId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public ApiKeyNotFoundException(ApiKeyId id, string propertyName) : base(BuildMessage(id, propertyName))
  {
    RealmId = id.RealmId?.ToGuid();
    ApiKeyId = id.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ApiKeyId id, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), id.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(ApiKeyId), id.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
