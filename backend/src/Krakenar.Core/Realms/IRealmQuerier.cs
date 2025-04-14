using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms;

public interface IRealmQuerier
{
  Task<RealmId?> FindIdAsync(Slug uniqueSlug, CancellationToken cancellationToken = default);

  Task<RealmDto> ReadAsync(Realm realm, CancellationToken cancellationToken = default);
  Task<RealmDto?> ReadAsync(RealmId id, CancellationToken cancellationToken = default);
  Task<RealmDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RealmDto?> ReadAsync(string uniqueSlug, CancellationToken cancellationToken = default);

  Task<SearchResults<RealmDto>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken = default);
}
