using Krakenar.Contracts.Senders;
using Krakenar.Core.Realms;

namespace Krakenar.Core.Senders;

public interface ISenderManager
{
  Task<Sender> FindAsync(string idOrKind, string propertyName, CancellationToken cancellationToken = default);
}

public class SenderManager : ISenderManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ISenderQuerier SenderQuerier { get; }
  protected virtual ISenderRepository SenderRepository { get; }

  public SenderManager(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    ApplicationContext = applicationContext;
    SenderQuerier = senderQuerier;
    SenderRepository = senderRepository;
  }

  public virtual async Task<Sender> FindAsync(string idOrKind, string propertyName, CancellationToken cancellationToken)
  {
    RealmId? realmId = ApplicationContext.RealmId;
    Sender? sender = null;

    if (Guid.TryParse(idOrKind, out Guid entityId))
    {
      SenderId senderId = new(entityId, realmId);
      sender = await SenderRepository.LoadAsync(senderId, cancellationToken);
    }

    if (sender is null && Enum.TryParse(idOrKind, out SenderKind kind))
    {
      SenderId? senderId = await SenderQuerier.FindDefaultIdAsync(kind, cancellationToken);
      if (senderId.HasValue)
      {
        sender = await SenderRepository.LoadAsync(senderId.Value, cancellationToken);
      }
    }

    return sender ?? throw new SenderNotFoundException(realmId, idOrKind, propertyName);
  }
}
