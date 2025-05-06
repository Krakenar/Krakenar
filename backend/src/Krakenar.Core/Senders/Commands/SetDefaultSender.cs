using Logitar.EventSourcing;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Commands;

public record SetDefaultSender(Guid Id) : ICommand<SenderDto?>;

public class SetDefaultSenderHandler : ICommandHandler<SetDefaultSender, SenderDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISenderQuerier SenderQuerier { get; }
  protected virtual ISenderRepository SenderRepository { get; }

  public SetDefaultSenderHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    ApplicationContext = applicationContext;
    SenderQuerier = senderQuerier;
    SenderRepository = senderRepository;
  }

  public virtual async Task<SenderDto?> HandleAsync(SetDefaultSender command, CancellationToken cancellationToken)
  {
    SenderId senderId = new(command.Id, ApplicationContext.RealmId);
    Sender? sender = await SenderRepository.LoadAsync(senderId, cancellationToken);
    if (sender is null)
    {
      return null;
    }

    if (!sender.IsDefault)
    {
      ActorId? actorId = ApplicationContext.ActorId;
      List<Sender> senders = new(capacity: 2);

      SenderId? defaultId = await SenderQuerier.FindDefaultIdAsync(sender.Kind, cancellationToken);
      if (defaultId.HasValue)
      {
        Sender @default = await SenderRepository.LoadAsync(defaultId.Value, cancellationToken) ?? throw new InvalidOperationException($"The sender 'Id={senderId}' was not loaded.");
        @default.SetDefault(isDefault: false, actorId);
        senders.Add(@default);
      }

      sender.SetDefault(isDefault: true, actorId);
      senders.Add(sender);

      await SenderRepository.SaveAsync(senders, cancellationToken);
    }

    return await SenderQuerier.ReadAsync(sender, cancellationToken);
  }
}
