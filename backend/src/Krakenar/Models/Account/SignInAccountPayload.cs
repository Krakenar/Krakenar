namespace Krakenar.Models.Account;

public record SignInAccountPayload
{
  public string Username { get; set; }
  public string Password { get; set; }

  public SignInAccountPayload() : this(string.Empty, string.Empty)
  {
  }

  public SignInAccountPayload(string username, string password)
  {
    Username = username;
    Password = password;
  }
}
