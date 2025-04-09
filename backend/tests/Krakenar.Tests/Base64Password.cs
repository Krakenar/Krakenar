using Krakenar.Core.Passwords;

namespace Krakenar;

public record Base64Password : Password
{
  private const string Prefix = "BASE64";

  private readonly string _password;

  public Base64Password(string password)
  {
    _password = password;
  }

  public override string Encode() => string.Join(Separator, Prefix, Convert.ToBase64String(Encoding.UTF8.GetBytes(_password)));

  public override bool IsMatch(string password) => _password.Equals(password);
}
