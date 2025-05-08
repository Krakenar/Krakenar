using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using Krakenar.Core.Messages.Commands;
using Krakenar.Core.Messages.Queries;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages;

public class MessageService : IMessageService
{
  protected virtual IQueryHandler<ReadMessage, MessageDto?> ReadMessage { get; }
  protected virtual IQueryHandler<SearchMessages, SearchResults<MessageDto>> SearchMessages { get; }
  protected virtual ICommandHandler<SendMessage, SentMessages> SendMessage { get; }

  public MessageService(
    IQueryHandler<ReadMessage, MessageDto?> readMessage,
    IQueryHandler<SearchMessages, SearchResults<MessageDto>> searchMessages,
    ICommandHandler<SendMessage, SentMessages> sendMessage)
  {
    ReadMessage = readMessage;
    SearchMessages = searchMessages;
    SendMessage = sendMessage;
  }

  public virtual async Task<MessageDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadMessage query = new(id);
    return await ReadMessage.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<MessageDto>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    SearchMessages query = new(payload);
    return await SearchMessages.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    SendMessage command = new(payload);
    return await SendMessage.HandleAsync(command, cancellationToken);
  }
}
