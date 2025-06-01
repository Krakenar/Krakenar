using Krakenar.Contracts;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Encryption;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Queries;

public record ReadSender(Guid? Id, SenderKind? Kind) : IQuery<SenderDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadSenderHandler : IQueryHandler<ReadSender, SenderDto?>
{
  protected virtual IEncryptionManager EncryptionManager { get; }
  protected virtual ISenderQuerier SenderQuerier { get; }

  public ReadSenderHandler(IEncryptionManager encryptionManager, ISenderQuerier senderQuerier)
  {
    EncryptionManager = encryptionManager;
    SenderQuerier = senderQuerier;
  }

  public virtual async Task<SenderDto?> HandleAsync(ReadSender query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, SenderDto> senders = new(capacity: 2);

    if (query.Id.HasValue)
    {
      SenderDto? sender = await SenderQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (sender is not null)
      {
        senders[sender.Id] = sender;
      }
    }

    if (query.Kind.HasValue)
    {
      SenderDto? sender = await SenderQuerier.ReadDefaultAsync(query.Kind.Value, cancellationToken);
      if (sender is not null)
      {
        senders[sender.Id] = sender;
      }
    }

    if (senders.Count > 1)
    {
      throw TooManyResultsException<SenderDto>.ExpectedSingle(senders.Count);
    }

    SenderDto? dto = senders.SingleOrDefault().Value;
    if (dto is not null)
    {
      EncryptionManager.DecryptSettings(dto);
    }
    return dto;
  }
}
