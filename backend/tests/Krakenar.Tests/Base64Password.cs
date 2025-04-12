using Krakenar.Core.Passwords;

namespace Krakenar;

public record Base64Password : Password
{
  public const string Key = "BASE64";

  private readonly string _password;

  public Base64Password(string password)
  {
    _password = password;
  }

  public static Base64Password Decode(string password)
  {
    string[] values = password.Split(Separator);
    if (values.Length != 2 || values.First() != Key)
    {
      throw new ArgumentException($"The value '{password}' is not a valid BASE64 password.", nameof(password));
    }

    return new Base64Password(Encoding.UTF8.GetString(Convert.FromBase64String(values.Last())));
  }

  public override string Encode() => string.Join(Separator, Key, Convert.ToBase64String(Encoding.UTF8.GetBytes(_password)));

  public override bool IsMatch(string password) => _password.Equals(password);
}
