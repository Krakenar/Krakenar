using Logitar.EventSourcing;

namespace Krakenar.Core.Senders;

public interface ISenderRepository
{
  Task<Sender?> LoadAsync(SenderId id, CancellationToken cancellationToken = default);
  Task<Sender?> LoadAsync(SenderId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Sender sender, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Sender> senders, CancellationToken cancellationToken = default);
}

public class SenderRepository : Repository, ISenderRepository
{
  public SenderRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<Sender?> LoadAsync(SenderId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<Sender?> LoadAsync(SenderId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Sender>(id.StreamId, version, cancellationToken);
  }

  public virtual async Task SaveAsync(Sender sender, CancellationToken cancellationToken)
  {
    await base.SaveAsync(sender, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<Sender> senders, CancellationToken cancellationToken)
  {
    await base.SaveAsync(senders, cancellationToken);
  }
}
