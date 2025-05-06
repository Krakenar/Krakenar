using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Commands;
using Krakenar.Core.Senders.Queries;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders;

public class SenderService : ISenderService
{
  protected virtual ICommandHandler<CreateOrReplaceSender, CreateOrReplaceSenderResult> CreateOrReplaceSender { get; }
  protected virtual ICommandHandler<DeleteSender, SenderDto?> DeleteSender { get; }
  protected virtual IQueryHandler<ReadSender, SenderDto?> ReadSender { get; }
  protected virtual IQueryHandler<SearchSenders, SearchResults<SenderDto>> SearchSenders { get; }
  protected virtual ICommandHandler<SetDefaultSender, SenderDto?> SetDefaultSender { get; }
  protected virtual ICommandHandler<UpdateSender, SenderDto?> UpdateSender { get; }

  public SenderService(
    ICommandHandler<CreateOrReplaceSender, CreateOrReplaceSenderResult> createOrReplaceSender,
    ICommandHandler<DeleteSender, SenderDto?> deleteSender,
    IQueryHandler<ReadSender, SenderDto?> readSender,
    IQueryHandler<SearchSenders, SearchResults<SenderDto>> searchSenders,
    ICommandHandler<SetDefaultSender, SenderDto?> setDefaultSender,
    ICommandHandler<UpdateSender, SenderDto?> updateSender)
  {
    CreateOrReplaceSender = createOrReplaceSender;
    DeleteSender = deleteSender;
    ReadSender = readSender;
    SearchSenders = searchSenders;
    SetDefaultSender = setDefaultSender;
    UpdateSender = updateSender;
  }

  public virtual async Task<CreateOrReplaceSenderResult> CreateOrReplaceAsync(CreateOrReplaceSenderPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceSender command = new(id, payload, version);
    return await CreateOrReplaceSender.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<SenderDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteSender command = new(id);
    return await DeleteSender.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<SenderDto?> ReadAsync(Guid? id, SenderKind? kind, CancellationToken cancellationToken)
  {
    ReadSender query = new(id, kind);
    return await ReadSender.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<SenderDto>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    SearchSenders query = new(payload);
    return await SearchSenders.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SenderDto?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    SetDefaultSender command = new(id);
    return await SetDefaultSender.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<SenderDto?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    UpdateSender command = new(id, payload);
    return await UpdateSender.HandleAsync(command, cancellationToken);
  }
}
