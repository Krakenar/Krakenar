using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Tokens;

public record CreateTokenPayload
{
  public bool IsConsumable { get; set; }

  public string? Algorithm { get; set; }
  public string? Audience { get; set; }
  public string? Issuer { get; set; }
  public int? LifetimeSeconds { get; set; }
  public string? Secret { get; set; }
  public string? Type { get; set; }

  public string? Subject { get; set; }
  public EmailPayload? Email { get; set; }
  public List<Claim> Claims { get; set; } = [];
}
