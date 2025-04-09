using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Sessions;

public record RefreshToken
{
  public const int SecretLength = 256 / 8;
  private const string Prefix = "RT";
  private const char Separator = '.';

  public SessionId SessionId { get; }
  public string Secret { get; }

  public RefreshToken(SessionId sessionId, string secret)
  {
    byte[] bytes = Convert.FromBase64String(secret);
    if (bytes.Length != SecretLength)
    {
      throw new ArgumentException($"The secret must be {SecretLength} bytes encoded as Base64.", nameof(secret));
    }

    SessionId = sessionId;
    Secret = secret;
  }

  public static RefreshToken Decode(string value, RealmId? realmId = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 3 || values[0] != Prefix)
    {
      throw new ArgumentException($"The value '{value}' is not a valid refresh token.", nameof(value));
    }

    Guid entityId = new(Convert.FromBase64String(values[1].FromUriSafeBase64()));
    SessionId sessionId = new(entityId, realmId);
    string secret = values[2].FromUriSafeBase64();
    return new RefreshToken(sessionId, secret);
  }

  public static string Encode(Session session, string secret) => Encode(session.Id, secret);
  public static string Encode(SessionId sessionId, string secret) => new RefreshToken(sessionId, secret).ToString();

  public override string ToString() => string.Join(Separator, Prefix,
    Convert.ToBase64String(SessionId.EntityId.ToByteArray()).ToUriSafeBase64(),
    Secret.ToUriSafeBase64());
}
