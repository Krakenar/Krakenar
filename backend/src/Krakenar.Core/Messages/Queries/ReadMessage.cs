using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages.Queries;

public record ReadMessage(Guid Id) : IQuery<MessageDto?>;

public class ReadMessageHandler : IQueryHandler<ReadMessage, MessageDto?>
{
  protected virtual IMessageQuerier MessageQuerier { get; }

  public ReadMessageHandler(IMessageQuerier messageQuerier)
  {
    MessageQuerier = messageQuerier;
  }

  public virtual async Task<MessageDto?> HandleAsync(ReadMessage query, CancellationToken cancellationToken)
  {
    return await MessageQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
