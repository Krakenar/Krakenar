using Krakenar.Core.Realms;

namespace Krakenar.Core.Tokens;

public interface ISecretService // TODO(fpion): implement
{
  Secret Generate(RealmId? realmId = null);
}
