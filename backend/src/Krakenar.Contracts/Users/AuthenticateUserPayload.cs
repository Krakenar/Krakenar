namespace Krakenar.Contracts.Users;

public record AuthenticateUserPayload
{
  public string User { get; set; }
  public string Password { get; set; }

  public AuthenticateUserPayload() : this(string.Empty, string.Empty)
  {
  }

  public AuthenticateUserPayload(string user, string password)
  {
    User = user;
    Password = password;
  }
}
