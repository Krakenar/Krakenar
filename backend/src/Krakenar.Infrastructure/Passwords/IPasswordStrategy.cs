using Krakenar.Core.Passwords;

namespace Krakenar.Infrastructure.Passwords;

public interface IPasswordStrategy
{
  string Key { get; }

  Password Decode(string password);
  Password Hash(string password);
}
