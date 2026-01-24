using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Commands;
using Krakenar.Core.Senders.Queries;
using Logitar.CQRS;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders;

public class SenderService : ISenderService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public SenderService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<CreateOrReplaceSenderResult> CreateOrReplaceAsync(CreateOrReplaceSenderPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceSender command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<SenderDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteSender command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<SenderDto?> ReadAsync(Guid? id, SenderKind? kind, CancellationToken cancellationToken)
  {
    ReadSender query = new(id, kind);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<SenderDto>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    SearchSenders query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SenderDto?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    SetDefaultSender command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<SenderDto?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    UpdateSender command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
