using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms.Queries;

public record SearchRealms(SearchRealmsPayload Payload) : IQuery<SearchResults<RealmDto>>;

public class SearchRealmsHandler : IQueryHandler<SearchRealms, SearchResults<RealmDto>>
{
  protected virtual IRealmQuerier RealmQuerier { get; }

  public SearchRealmsHandler(IRealmQuerier realmQuerier)
  {
    RealmQuerier = realmQuerier;
  }

  public virtual async Task<SearchResults<RealmDto>> HandleAsync(SearchRealms query, CancellationToken cancellationToken)
  {
    return await RealmQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
