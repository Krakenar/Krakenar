using Krakenar.Core.Encryption;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages.Queries;

public record ReadMessage(Guid Id) : IQuery<MessageDto?>;

public class ReadMessageHandler : IQueryHandler<ReadMessage, MessageDto?>
{
  protected virtual IEncryptionManager EncryptionManager { get; }
  protected virtual IMessageQuerier MessageQuerier { get; }

  public ReadMessageHandler(IEncryptionManager encryptionManager, IMessageQuerier messageQuerier)
  {
    EncryptionManager = encryptionManager;
    MessageQuerier = messageQuerier;
  }

  public virtual async Task<MessageDto?> HandleAsync(ReadMessage query, CancellationToken cancellationToken)
  {
    MessageDto? message = await MessageQuerier.ReadAsync(query.Id, cancellationToken);
    if (message is not null)
    {
      EncryptionManager.Decrypt(message);
    }
    return message;
  }
}
