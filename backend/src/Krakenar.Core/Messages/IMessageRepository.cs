namespace Krakenar.Core.Messages;

public interface IMessageRepository
{
  Task SaveAsync(Message message, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default);
}
