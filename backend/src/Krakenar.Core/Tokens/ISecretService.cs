using Krakenar.Core.Realms;

namespace Krakenar.Core.Tokens;

public interface ISecretService // TODO(fpion): implement
{
  Secret Encrypt(string secret, RealmId? realmId = null);
  Secret Generate(RealmId? realmId = null);
}
