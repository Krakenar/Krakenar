using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Senders;

public interface ISenderService
{
  Task<CreateOrReplaceSenderResult> CreateOrReplaceAsync(CreateOrReplaceSenderPayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<Sender?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> ReadAsync(Guid? id = null, SenderKind? kind = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken = default);
  Task<Sender?> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Sender?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken = default);
}
