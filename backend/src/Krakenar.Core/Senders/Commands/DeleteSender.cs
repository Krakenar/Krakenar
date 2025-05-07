using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Commands;

public record DeleteSender(Guid Id) : ICommand<SenderDto?>;

public class DeleteSenderHandler : ICommandHandler<DeleteSender, SenderDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISenderQuerier SenderQuerier { get; }
  protected virtual ISenderRepository SenderRepository { get; }

  public DeleteSenderHandler(
    IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    ApplicationContext = applicationContext;
    SenderQuerier = senderQuerier;
    SenderRepository = senderRepository;
  }

  public virtual async Task<SenderDto?> HandleAsync(DeleteSender command, CancellationToken cancellationToken)
  {
    SenderId senderId = new(command.Id, ApplicationContext.RealmId);
    Sender? sender = await SenderRepository.LoadAsync(senderId, cancellationToken);
    if (sender is null)
    {
      return null;
    }
    SenderDto dto = await SenderQuerier.ReadAsync(sender, cancellationToken);

    // TODO(fpion): you should not be able to delete a default sender unless its the only sender.

    sender.Delete(ApplicationContext.ActorId);
    await SenderRepository.SaveAsync(sender, cancellationToken);

    return dto;
  }
}
