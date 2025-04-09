using Krakenar.Core.Realms;
using Krakenar.Core.Tokens;

namespace Krakenar.Infrastructure.Tokens;

public class SecretService : ISecretService // TODO(fpion): implement
{
  public virtual Secret Encrypt(string secret, RealmId? realmId)
  {
    throw new NotImplementedException();
  }

  public virtual Secret Generate(RealmId? realmId = null)
  {
    throw new NotImplementedException();
  }
}
