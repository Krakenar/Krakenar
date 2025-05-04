using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.ApiKeys;

public class ApiKeyIsExpiredException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified API key is expired.";

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

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(ApiKeyId)] = ApiKeyId;
      return error;
    }
  }

  public ApiKeyIsExpiredException(ApiKey apiKey) : base(BuildMessage(apiKey))
  {
    RealmId = apiKey.RealmId?.ToGuid();
    ApiKeyId = apiKey.EntityId;
  }

  private static string BuildMessage(ApiKey apiKey) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), apiKey.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(ApiKeyId), apiKey.EntityId)
    .Build();
}
