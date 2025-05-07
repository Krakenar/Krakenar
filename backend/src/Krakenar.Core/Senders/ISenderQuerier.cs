using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders;

public interface ISenderQuerier
{
  Task<SenderId?> FindDefaultIdAsync(SenderKind kind, CancellationToken cancellationToken = default);

  Task<SenderDto> ReadAsync(Sender sender, CancellationToken cancellationToken = default);
  Task<SenderDto?> ReadAsync(SenderId id, CancellationToken cancellationToken = default);
  Task<SenderDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SenderDto?> ReadDefaultAsync(SenderKind kind, CancellationToken cancellationToken = default);

  Task<SearchResults<SenderDto>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken = default);
}
