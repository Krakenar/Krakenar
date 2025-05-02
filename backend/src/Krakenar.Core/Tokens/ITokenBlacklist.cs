namespace Krakenar.Core.Tokens;

public interface ITokenBlacklist
{
  Task BlacklistAsync(IEnumerable<string> tokenIds, CancellationToken cancellationToken = default);
  Task BlacklistAsync(IEnumerable<string> tokenIds, DateTime? expiresOn, CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<string>> GetBlacklistedAsync(IEnumerable<string> tokenIds, CancellationToken cancellationToken = default);

  Task PurgeAsync(CancellationToken cancellationToken = default);
}
