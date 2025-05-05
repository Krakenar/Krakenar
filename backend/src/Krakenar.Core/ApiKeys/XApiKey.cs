using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.ApiKeys;

public record XApiKey
{
  public const int SecretLength = 256 / 8;
  private const string Prefix = "KK";
  private const char Separator = '.';

  public ApiKeyId ApiKeyId { get; }
  public string Secret { get; }

  public XApiKey(ApiKeyId apiKeyId, string secret)
  {
    byte[] bytes = Convert.FromBase64String(secret);
    if (bytes.Length != SecretLength)
    {
      throw new ArgumentException($"The secret must be {SecretLength} bytes encoded as Base64.", nameof(secret));
    }

    ApiKeyId = apiKeyId;
    Secret = secret;
  }

  public static XApiKey Decode(string value, RealmId? realmId = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 3 || values[0] != Prefix)
    {
      throw new ArgumentException($"The value '{value}' is not a valid refresh token.", nameof(value));
    }

    Guid entityId = new(Convert.FromBase64String(values[1].FromUriSafeBase64()));
    ApiKeyId apiKeyId = new(entityId, realmId);
    string secret = values[2].FromUriSafeBase64();
    return new XApiKey(apiKeyId, secret);
  }

  public static string Encode(ApiKey apiKey, string secret) => Encode(apiKey.Id, secret);
  public static string Encode(ApiKeyId apiKeyId, string secret) => new XApiKey(apiKeyId, secret).ToString();

  public override string ToString() => string.Join(Separator, Prefix,
    Convert.ToBase64String(ApiKeyId.EntityId.ToByteArray()).ToUriSafeBase64(),
    Secret.ToUriSafeBase64());
}
