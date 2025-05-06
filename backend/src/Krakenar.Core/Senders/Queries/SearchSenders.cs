using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Queries;

public record SearchSenders(SearchSendersPayload Payload) : IQuery<SearchResults<SenderDto>>;

public class SearchSendersHandler : IQueryHandler<SearchSenders, SearchResults<SenderDto>>
{
  protected virtual ISenderQuerier SenderQuerier { get; }

  public SearchSendersHandler(ISenderQuerier senderQuerier)
  {
    SenderQuerier = senderQuerier;
  }

  public virtual async Task<SearchResults<SenderDto>> HandleAsync(SearchSenders query, CancellationToken cancellationToken)
  {
    return await SenderQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
