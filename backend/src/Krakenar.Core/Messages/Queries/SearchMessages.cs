using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages.Queries;

public record SearchMessages(SearchMessagesPayload Payload) : IQuery<SearchResults<MessageDto>>;

public class SearchMessagesHandler : IQueryHandler<SearchMessages, SearchResults<MessageDto>>
{
  protected virtual IMessageQuerier MessageQuerier { get; }

  public SearchMessagesHandler(IMessageQuerier messageQuerier)
  {
    MessageQuerier = messageQuerier;
  }

  public virtual async Task<SearchResults<MessageDto>> HandleAsync(SearchMessages query, CancellationToken cancellationToken)
  {
    return await MessageQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
