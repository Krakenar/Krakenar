using Krakenar.Contracts.Settings;
using Krakenar.Core.Passwords;

namespace Krakenar.Infrastructure.Passwords;

public class PasswordService : IPasswordService // TODO(fpion): implement
{
  public Password GenerateBase64(int length, out string password)
  {
    throw new NotImplementedException();
  }

  public Password ValidateAndHash(string password, IPasswordSettings? settings)
  {
    throw new NotImplementedException();
  }
}
