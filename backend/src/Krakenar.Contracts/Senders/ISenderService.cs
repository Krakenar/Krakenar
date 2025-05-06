namespace Krakenar.Contracts.Senders;

public interface ISenderService
{
  Task<CreateOrReplaceSenderResult> CreateOrReplaceAsync(CreateOrReplaceSenderPayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
}
