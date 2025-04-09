using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Passwords;

public interface IPasswordService
{
  Password GenerateBase64(int length, out string password);
  Password ValidateAndHash(string password, IPasswordSettings? settings = null);
}
