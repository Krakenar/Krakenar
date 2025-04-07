using Krakenar.Contracts.Settings;

namespace Krakenar.Core.Passwords;

public interface IPasswordService // TODO(fpion): implement
{
  Password ValidateAndHash(string password, IPasswordSettings? settings = null);
}
