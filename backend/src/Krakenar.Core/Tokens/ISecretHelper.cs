using Krakenar.Core.Realms;

namespace Krakenar.Core.Tokens;

public interface ISecretHelper
{
  Secret Generate(RealmId? realmId = null);
}
