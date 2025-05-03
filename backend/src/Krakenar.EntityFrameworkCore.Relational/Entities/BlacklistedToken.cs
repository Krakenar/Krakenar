namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class BlacklistedToken
{
  public int BlacklistedTokenId { get; private set; }

  public string TokenId { get; private set; }

  public DateTime? ExpiresOn { get; set; }

  public BlacklistedToken(string tokenId)
  {
    TokenId = tokenId;
  }

  private BlacklistedToken() : this(string.Empty)
  {
  }

  public override bool Equals(object? obj) => obj is BlacklistedToken blacklisted && blacklisted.TokenId == TokenId;
  public override int GetHashCode() => TokenId.GetHashCode();
  public override string ToString() => $"{GetType()} (TokenId={TokenId})";
}
