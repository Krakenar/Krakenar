using Krakenar.Core.Realms;
using Logitar;

namespace Krakenar.Core.Sessions;

public record RefreshToken
{
  public const int SecretLength = 256 / 8;
  private const string Prefix = "RT";
  private const char Separator = ':';

  public SessionId SessionId { get; }
  public string Secret { get; }

  private RefreshToken(SessionId sessionId, string secret)
  {
    if (Convert.FromBase64String(secret).Length != SecretLength)
    {
      throw new ArgumentException($"The secret length must be {SecretLength}.", nameof(secret));
    }

    SessionId = sessionId;
    Secret = secret;
  }

  public static RefreshToken Decode(string value, RealmId? realmId = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 3 || values.First() != Prefix)
    {
      throw new ArgumentException($"The value '{value}' is not a valid refresh token.", nameof(value));
    }

    Guid entityId = new(Convert.FromBase64String(values[1].FromUriSafeBase64()));
    SessionId sessionId = new(entityId, realmId);
    return new RefreshToken(sessionId, values[2].FromUriSafeBase64());
  }

  public static string Encode(Session session, string secret) => new RefreshToken(session.Id, secret).ToString();

  public override string ToString() => string.Join(Separator, Prefix,
    Convert.ToBase64String(SessionId.EntityId.ToByteArray()).ToUriSafeBase64(),
    Secret.ToUriSafeBase64());
}
