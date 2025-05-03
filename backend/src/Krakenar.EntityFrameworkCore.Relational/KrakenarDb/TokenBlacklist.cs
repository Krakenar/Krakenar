using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class TokenBlacklist
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.TokenBlacklist), alias: null);

  public static readonly ColumnId BlacklistedTokenId = new(nameof(BlacklistedToken.BlacklistedTokenId), Table);
  public static readonly ColumnId ExpiresOn = new(nameof(BlacklistedToken.ExpiresOn), Table);
  public static readonly ColumnId TokenId = new(nameof(BlacklistedToken.TokenId), Table);
}
