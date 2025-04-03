using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Passwords;

public interface IPasswordManager
{
  Password ValidateAndHash(IPasswordSettings settings, string password);
}
