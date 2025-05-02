namespace Krakenar.Contracts.Tokens;

public record CreatedToken
{
  public string Token { get; set; } = string.Empty;

  public CreatedToken()
  {
  }

  public CreatedToken(string token)
  {
    Token = token;
  }
}
