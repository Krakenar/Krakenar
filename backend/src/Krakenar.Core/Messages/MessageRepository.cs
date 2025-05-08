using Logitar.EventSourcing;

namespace Krakenar.Core.Messages;

public interface IMessageRepository
{
  Task<Message?> LoadAsync(MessageId messageId, CancellationToken cancellationToken = default);
  Task<Message?> LoadAsync(MessageId messageId, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Message message, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default);
}

public class MessageRepository : Repository, IMessageRepository
{
  public MessageRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<Message?> LoadAsync(MessageId messageId, CancellationToken cancellationToken)
  {
    return await LoadAsync(messageId, version: null, cancellationToken);
  }
  public virtual async Task<Message?> LoadAsync(MessageId messageId, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Message>(messageId.StreamId, version, cancellationToken);
  }

  public virtual async Task SaveAsync(Message message, CancellationToken cancellationToken)
  {
    await base.SaveAsync(message, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<Message> messages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(messages, cancellationToken);
  }
}
