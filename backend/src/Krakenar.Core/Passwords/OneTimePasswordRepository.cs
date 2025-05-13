using Logitar.EventSourcing;

namespace Krakenar.Core.Passwords;

public interface IOneTimePasswordRepository
{
  Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, CancellationToken cancellationToken = default);
  Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, long? version, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(IEnumerable<OneTimePasswordId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<OneTimePassword> oneTimePasswords, CancellationToken cancellationToken = default);
}

public class OneTimePasswordRepository : Repository, IOneTimePasswordRepository
{
  public OneTimePasswordRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<OneTimePassword?> LoadAsync(OneTimePasswordId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<OneTimePassword>(id.StreamId, version, cancellationToken);
  }
  public virtual async Task<IReadOnlyCollection<OneTimePassword>> LoadAsync(IEnumerable<OneTimePasswordId> ids, CancellationToken cancellationToken)
  {
    return await LoadAsync<OneTimePassword>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public virtual async Task SaveAsync(OneTimePassword onetimepassword, CancellationToken cancellationToken)
  {
    await base.SaveAsync(onetimepassword, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<OneTimePassword> onetimepasswords, CancellationToken cancellationToken)
  {
    await base.SaveAsync(onetimepasswords, cancellationToken);
  }
}
