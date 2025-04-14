using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using SessionDto = Krakenar.Contracts.Sessions.Session;

namespace Krakenar.Core.Sessions.Queries;

public record SearchSessions(SearchSessionsPayload Payload) : IQuery<SearchResults<SessionDto>>;

public class SearchSessionsHandler : IQueryHandler<SearchSessions, SearchResults<SessionDto>>
{
  protected virtual ISessionQuerier SessionQuerier { get; }

  public SearchSessionsHandler(ISessionQuerier sessionQuerier)
  {
    SessionQuerier = sessionQuerier;
  }

  public virtual async Task<SearchResults<SessionDto>> HandleAsync(SearchSessions query, CancellationToken cancellationToken)
  {
    return await SessionQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
