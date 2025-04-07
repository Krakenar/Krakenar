using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms;

public interface IRealmQuerier
{
  Task<RealmDto> ReadAsync(Realm realm, CancellationToken cancellationToken = default);
  Task<RealmDto?> ReadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<RealmDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmDto?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken = default);
}
