using Logitar.EventSourcing;

namespace Krakenar.Core.Dictionaries;

public interface IDictionaryRepository
{
  Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Dictionary> dictionaries, CancellationToken cancellationToken = default);
}

public class DictionaryRepository : Repository, IDictionaryRepository
{
  public DictionaryRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken)
  {
    await base.SaveAsync(dictionary, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<Dictionary> dictionaries, CancellationToken cancellationToken)
  {
    await base.SaveAsync(dictionaries, cancellationToken);
  }
}
