namespace Krakenar.Core.Tokens;

public record CreateTokenOptions
{
  public string Type { get; set; } = "JWT";
  public string SigningAlgorithm { get; set; } = "HS256";

  public string? Audience { get; set; }
  public string? Issuer { get; set; }

  public DateTime? Expires { get; set; }
  public DateTime? IssuedAt { get; set; }
  public DateTime? NotBefore { get; set; }
}
