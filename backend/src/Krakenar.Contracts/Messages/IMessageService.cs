namespace Krakenar.Contracts.Messages;

public interface IMessageService
{
  Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default);
}
