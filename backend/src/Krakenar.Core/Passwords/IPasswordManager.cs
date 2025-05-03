using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Passwords;

public interface IPasswordManager
{
  Password Decode(string password);

  Password Generate(int length, out string password);
  Password Generate(string characters, int length, out string password);

  Password GenerateBase64(int length, out string password);

  Password Hash(string password);

  void Validate(string password);

  Password ValidateAndHash(string password, IPasswordSettings? settings = null);
}
