using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Passwords;

public interface IPasswordService // TODO(fpion): implement
{
  Password GenerateBase64(int length, out string password);
  Password ValidateAndHash(string password, IPasswordSettings? settings = null);
}
