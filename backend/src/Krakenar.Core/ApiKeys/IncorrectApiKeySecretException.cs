using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.ApiKeys;

public class IncorrectApiKeySecretException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified secret did not match the API key.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)]!;
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid ApiKeyId
  {
    get => (Guid)Data[nameof(ApiKeyId)]!;
    private set => Data[nameof(ApiKeyId)] = value;
  }
  public string AttemptedSecret
  {
    get => (string)Data[nameof(AttemptedSecret)]!;
    private set => Data[nameof(AttemptedSecret)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RealmId)] = RealmId;
      error.Data[nameof(ApiKeyId)] = ApiKeyId;
      error.Data[nameof(AttemptedSecret)] = AttemptedSecret;
      return error;
    }
  }

  public IncorrectApiKeySecretException(ApiKey apiKey, string attemptedSecret)
    : base(BuildMessage(apiKey, attemptedSecret))
  {
    RealmId = apiKey.RealmId?.ToGuid();
    ApiKeyId = apiKey.EntityId;
    AttemptedSecret = attemptedSecret;
  }

  private static string BuildMessage(ApiKey apiKey, string attemptedSecret) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), apiKey.RealmId?.ToGuid(), "<null>")
    .AddData(nameof(ApiKeyId), apiKey.EntityId)
    .AddData(nameof(AttemptedSecret), attemptedSecret)
    .Build();
}
