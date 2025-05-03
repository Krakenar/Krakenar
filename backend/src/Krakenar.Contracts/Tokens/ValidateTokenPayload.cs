namespace Krakenar.Contracts.Tokens;

public record ValidateTokenPayload
{
  public string Token { get; set; } = string.Empty;
  public bool Consume { get; set; }

  public string? Audience { get; set; }
  public string? Issuer { get; set; }
  public string? Secret { get; set; }
  public string? Type { get; set; }

  public ValidateTokenPayload()
  {
  }

  public ValidateTokenPayload(string token)
  {
    Token = token;
  }
}
