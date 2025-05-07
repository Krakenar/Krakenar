using Krakenar.Contracts.Messages;
using Krakenar.Core.Messages.Commands;
using Krakenar.Core.Messages.Queries;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages;

public class MessageService : IMessageService
{
  protected virtual IQueryHandler<ReadMessage, MessageDto?> ReadMessage { get; }
  protected virtual ICommandHandler<SendMessage, SentMessages> SendMessage { get; }

  public MessageService(
    IQueryHandler<ReadMessage, MessageDto?> readMessage,
    ICommandHandler<SendMessage, SentMessages> sendMessage)
  {
    ReadMessage = readMessage;
    SendMessage = sendMessage;
  }

  public virtual async Task<MessageDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadMessage query = new(id);
    return await ReadMessage.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    SendMessage command = new(payload);
    return await SendMessage.HandleAsync(command, cancellationToken);
  }
}
