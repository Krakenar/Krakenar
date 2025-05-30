using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Realms;

public interface IRealmService
{
  Task<CreateOrReplaceRealmResult> CreateOrReplaceAsync(CreateOrReplaceRealmPayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<Realm?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Realm?> ReadAsync(Guid? id = null, string? uniqueSlug = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Realm>> SearchAsync(SearchRealmsPayload payload, CancellationToken cancellationToken = default);
  Task<Realm?> UpdateAsync(Guid id, UpdateRealmPayload payload, CancellationToken cancellationToken = default);
}
