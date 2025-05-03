using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Tokens;

public record ValidatedToken
{
  public string? Subject { get; set; }
  public Email? Email { get; set; }
  public List<Claim> Claims { get; set; } = [];
}
