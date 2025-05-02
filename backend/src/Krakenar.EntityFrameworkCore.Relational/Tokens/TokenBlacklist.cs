using Krakenar.Core.Tokens;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar;
using Microsoft.EntityFrameworkCore;

namespace Krakenar.EntityFrameworkCore.Relational.Tokens;

public class TokenBlacklist : ITokenBlacklist
{
  protected virtual KrakenarContext Context { get; }

  public TokenBlacklist(KrakenarContext context)
  {
    Context = context;
  }

  public virtual async Task BlacklistAsync(IEnumerable<string> tokenIds, CancellationToken cancellationToken)
  {
    await BlacklistAsync(tokenIds, expiresOn: null, cancellationToken);
  }
  public virtual async Task BlacklistAsync(IEnumerable<string> tokenIds, DateTime? expiresOn, CancellationToken cancellationToken)
  {
    expiresOn = expiresOn?.AsUniversalTime();

    Dictionary<string, BlacklistedToken> entities = await Context.TokenBlacklist
      .Where(x => tokenIds.Contains(x.TokenId))
      .ToDictionaryAsync(x => x.TokenId, x => x, cancellationToken);

    foreach (string tokenId in tokenIds)
    {
      if (!entities.TryGetValue(tokenId, out BlacklistedToken? entity))
      {
        entity = new(tokenId);
        entities[tokenId] = entity;

        Context.TokenBlacklist.Add(entity);
      }

      entity.ExpiresOn = expiresOn;
    }

    await Context.SaveChangesAsync(cancellationToken);
  }

  public virtual async Task<IReadOnlyCollection<string>> GetBlacklistedAsync(IEnumerable<string> tokenIds, CancellationToken cancellationToken)
  {
    DateTime now = DateTime.UtcNow;

    string[] blacklistedTokenIds = await Context.TokenBlacklist.AsNoTracking()
      .Where(x => tokenIds.Contains(x.TokenId) && (x.ExpiresOn == null || x.ExpiresOn > now))
      .Select(x => x.TokenId)
      .ToArrayAsync(cancellationToken);

    return blacklistedTokenIds;
  }

  public virtual async Task PurgeAsync(CancellationToken cancellationToken)
  {
    DateTime now = DateTime.UtcNow;

    BlacklistedToken[] expiredEntities = await Context.TokenBlacklist
      .Where(x => x.ExpiresOn != null && x.ExpiresOn <= now)
      .ToArrayAsync(cancellationToken);

    Context.TokenBlacklist.RemoveRange(expiredEntities);
    await Context.SaveChangesAsync(cancellationToken);
  }
}
