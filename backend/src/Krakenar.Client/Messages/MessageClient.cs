using Krakenar.Contracts.Messages;

namespace Krakenar.Client.Messages;

public class MessageClient : BaseClient, IMessageService
{
  protected virtual Uri Path { get; } = new("/api/messages", UriKind.Relative);

  public MessageClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await GetAsync<Message>(uri, cancellationToken)).Value;
  }

  public virtual async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    return (await PostAsync<SentMessages>(Path, payload, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SendAsync), HttpMethod.Post, Path, payload);
  }
}
