namespace Krakenar.Contracts.Users;

public record ResetUserPasswordPayload
{
  public string Password { get; set; }

  public ResetUserPasswordPayload() : this(string.Empty)
  {
  }

  public ResetUserPasswordPayload(string password)
  {
    Password = password;
  }
}
