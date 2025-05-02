using Krakenar.Core.Realms;

namespace Krakenar.Core.Tokens;

public interface ISecretManager
{
  string Decrypt(Secret secret, RealmId? realmId = null);
  Secret Encrypt(string secret, RealmId? realmId = null);
  Secret Generate(RealmId? realmId = null);
  string Resolve(string? secret);
}
