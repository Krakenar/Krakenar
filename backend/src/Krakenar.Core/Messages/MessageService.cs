using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using Krakenar.Core.Messages.Commands;
using Krakenar.Core.Messages.Queries;
using Logitar.CQRS;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages;

public class MessageService : IMessageService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public MessageService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<MessageDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadMessage query = new(id);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<MessageDto>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    SearchMessages query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    SendMessage command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
